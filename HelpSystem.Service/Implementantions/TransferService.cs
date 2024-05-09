using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{

    public class TransferService : ITransferService
    {
        private readonly IBaseRepository<ProductMovement> _transBaseRepository;
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        public TransferService(IBaseRepository<ProductMovement> movement, IBaseRepository<Products> productsRepository, IBaseRepository<Warehouse> warehouse)
        {
            _transBaseRepository = movement;
            _productsRepository = productsRepository;
            _warehouseRepository = warehouse;
        }

        public async Task<BaseResponse<IEnumerable<ProductMovement>>> AddTransferService(List<TransferViewModel> model)
        {
            try
            {
                // Проверяем, что список model не является пустым
                if (model.Count == 0)
                {
                    return new BaseResponse<IEnumerable<ProductMovement>>()
                    {
                        Description = "Список товаров для перемещения пуст.",
                        StatusCode = StatusCode.NotFind,
                    };
                }


                //Неважно одно это перемещение или множественное, на вход всё равно подается список
                //если там 1 товар, то он и будет один записан, а если там больше одного, так так и будет

                var products = new List<Products>(); // Создаем список товаров для перемещения

                foreach (var prod in model)
                {

                    var productList = await _productsRepository.GetAll()
                        .Include(w => w.Warehouse)
                        .Where(x => x.Id == prod.Id)
                        .Where(x => x.UserId == null)
                        .ToListAsync();

                    products.AddRange(productList); // Добавляем товары в общий список
                }

                // Проверяем, что есть доступные товары для перемещения
                if (!products.Any())
                {
                    return new BaseResponse<IEnumerable<ProductMovement>>()
                    {
                        Description = "Невозможно списать товар, который закреплён за пользователем.",
                        StatusCode = StatusCode.NotFind,
                    };
                }

                int totalTransferCount = model.Sum(x => x.CountTransfer); // Общее количество перемещаемых товаров

                // Проверяем, что запрошенное количество товаров доступно для перемещения
                if (totalTransferCount > products.Count)
                {
                    return new BaseResponse<IEnumerable<ProductMovement>>()
                    {
                        Description = "Запрошено больше количество перемещаемых товаров, чем доступно.",
                        StatusCode = StatusCode.UnMove
                    };
                }

                // Получаем id целевого склада
                Guid destinationWarehouseId = model.First().DestinationWarehouseId;
                if (destinationWarehouseId == Guid.Empty)
                {
                    return new BaseResponse<IEnumerable<ProductMovement>>
                    {
                        Description = "Перемещаемый склад не найден.",
                        StatusCode = StatusCode.NotFind,
                    };
                }


                var UtilizationBd = await _warehouseRepository.GetAll()
                    .Where(x => x.Id == destinationWarehouseId && x.IsService)
                    .FirstOrDefaultAsync();

                if (UtilizationBd != null)
                {
                    //сделаю функцию, которая перед записью о перемемещении товара добавит всем товарам время по списанию
                    await MarkProduct(products,model);

                }


                Guid SourceWarehouseID = model.First().SourceWarehouseId;
                // Выполняем операцию перемещения для указанного количества товаров
                foreach (var transfer in model)
                {
                    var transferCount = transfer.CountTransfer; // Количество перемещаемых товаров из текущей модели
                    // Наименование товара из текущей модели
                    var TransferId = transfer.Id;
                    // Получаем список товаров для текущей модели
                    var productsToTransfer = products.Where(p => p.Id == TransferId);

                    foreach (var product in productsToTransfer)
                    {
                        // Получаем текущее местоположение товара
                        var currentPositionResponse = await GetCurrentPositionProduct(product.Id);
                        Guid sourceWarehouseId;

                        if (currentPositionResponse.StatusCode == StatusCode.Ok)
                        {
                            sourceWarehouseId = currentPositionResponse.Data.DestinationWarehouseId;
                            SourceWarehouseID = sourceWarehouseId;
                        }
                        else
                        {
                            sourceWarehouseId = product.Warehouse.Id; // Если информация о перемещениях отсутствует, берем исходный склад
                        }



                        // Создаем запись о перемещении товара
                        var movement = new ProductMovement()
                        {
                            ProductId = product.Id,
                            SourceWarehouseId = sourceWarehouseId,
                            DestinationWarehouseId = destinationWarehouseId,
                            MovementDate = DateTime.Now,
                            Comments = transfer.Comments
                        };
                        await _transBaseRepository.Create(movement);
                    }
                }

                // Составляем описание операции перемещения
                //Склад на который переместили

                //Вот такие небольшие запросики :)
                var NameWarehouse = await _warehouseRepository.GetAll()
                    .Where(x => x.Id == destinationWarehouseId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync();

                //Склад с которого, когда указывал изначальные, я забыл о том, что я запросом вытягиваю их положение изначально

                var SourceName = await _warehouseRepository.GetAll()
                  .Where(x => x.Id == SourceWarehouseID)
                  .Select(x => x.Name)
                  .FirstOrDefaultAsync();

                string destinationWarehouseName = NameWarehouse;
                int lastDigit = totalTransferCount % 10;
                string description;
                if (totalTransferCount >= 11 && totalTransferCount <= 14)
                {
                    description = $"{totalTransferCount} товаров было перемещено с {SourceName} на {destinationWarehouseName}";
                }
                else if (lastDigit == 1)
                {
                    description = $"{totalTransferCount} товар был перемещён с {SourceName} на {destinationWarehouseName}";
                }
                else if (lastDigit >= 2 && lastDigit <= 4)
                {
                    description = $"{totalTransferCount} товара было перемещено с {SourceName} на {destinationWarehouseName}";
                }
                else
                {
                    description = $"{totalTransferCount} товаров было перемещено с {SourceName} на {destinationWarehouseName}";
                }

                return new BaseResponse<IEnumerable<ProductMovement>>()
                {
                    StatusCode = StatusCode.Ok,
                    Description = description
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<ProductMovement>>()
                {
                    Description = $"Произошла ошибка при перемещении товаров: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        /// <summary>
        /// Метод удаления записи и списания товара
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<ProductMovement>> DeleteTransferService(int ProductId)
        {
            try
            {
                var DebitingProduct = await _productsRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == ProductId);
                if (DebitingProduct != null) {
                    //Находим последнюю запись это на склад утилизации и удаляем ее, очищая время.
                    var LastMovement = await _transBaseRepository.GetAll()
                        .Where(x => x.Product == DebitingProduct)
                        //Чтобы найти последнюю, отсортируем по времени
                        .OrderByDescending(x=>x.MovementDate)
                        .FirstOrDefaultAsync();
                    if (LastMovement != null)
                    {
                        DebitingProduct.TimeDebbiting = null;
                        await _transBaseRepository.Delete(LastMovement);
                        await _productsRepository.Update(DebitingProduct);
                        return new BaseResponse<ProductMovement>()
                        {
                            StatusCode = StatusCode.Ok,
                            Description = $"{DebitingProduct.NameProduct} {DebitingProduct.InventoryCode} был восстановлен"
                        };
                    }

                    return new BaseResponse<ProductMovement>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = $"Перемещение на склад утилизации не найдено",
                    };
                   
                   
                }
                return new BaseResponse<ProductMovement>()
                {
                    StatusCode = StatusCode.NotFind,
                    Description = $"Товар не найден"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductMovement>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = ex.Message
                };
              
            }
        }

        public async Task<BaseResponse<ProductMovement>> GetCurrentPositionProduct(int ProductId)
        {
            try
            {
                //Получаем последнее перемещение товара
                var LatesMovement = await _transBaseRepository.GetAll()
                    .OrderByDescending(m => m.MovementDate)
                    .FirstOrDefaultAsync(m => m.ProductId == ProductId);
                if (LatesMovement != null)
                {
                    //Получаем наименование склада по Id
                    var WhName = await _warehouseRepository.GetAll()
                        .Where(x => x.Id == LatesMovement.DestinationWarehouseId)
                        .Select(n => n.Name)
                        .FirstOrDefaultAsync();
                    //Пока просто верну инфу о том, где сейчас товар
                    return new BaseResponse<ProductMovement>()
                    {
                        Data = LatesMovement,
                        Description = $" {WhName}",
                        StatusCode = StatusCode.Ok
                    };
                }
                return new BaseResponse<ProductMovement>()
                {
                    StatusCode = StatusCode.NotFind,
                    Description = "Информация о перемещениях данного товара отсутствует"
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductMovement>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }

        }
        //Метод для отметки списание товара
        private async Task MarkProduct(List<Products> product,List<TransferViewModel>mod)
        {
            foreach (var prod in product)
            {
                prod.TimeDebbiting = DateTime.Now;
            
            }

            for (int i = 0; i < Math.Min(product.Count, mod.Count); i++){
                product[i].CommentDebbiting = mod[i].CommentDebiting;
                await _productsRepository.Update(product[i]);
            }
        }


    }
}
