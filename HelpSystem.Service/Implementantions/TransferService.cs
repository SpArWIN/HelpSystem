using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class TransferService :ITransferService
    {
        private readonly IBaseRepository<ProductMovement> _transBaseRepository;
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        public TransferService(IBaseRepository<ProductMovement> movement, IBaseRepository<Products> productsRepository,IBaseRepository<Warehouse> warehouse)
        {
            _transBaseRepository = movement;
            _productsRepository = productsRepository;
            _warehouseRepository = warehouse;
        }
        //todo ПРИЕХАТЬ И ДОПИСАТЬ МЕТОД, СО СКЛАДОМ, ЧТОБЫ БЫЛА КОЛЛЕЦИЯ
        public async Task<BaseResponse<IEnumerable<ProductMovement>>> AddTransferService(List<TransferViewModel> model)
        {
            try
            {
                // Проверяем, что список model не является пустым
                if ( model.Count == 0)
                {
                    return new BaseResponse<IEnumerable<ProductMovement>>()
                    {
                        Description = "Список товаров для перемещения пуст.",
                        StatusCode = StatusCode.NotFind,
                    };
                }


                if (model.Count == 1)
                {
                    Guid productId = model.First().Id;
                    
                    //Находим товар в бд
                    var ProductMove = await _productsRepository.GetAll()
                        .Include(w=>w.Warehouse)
                        .FirstOrDefaultAsync(x => x.Id == productId);
                    if (ProductMove == null)
                    {
                        return new BaseResponse<IEnumerable<ProductMovement>>()
                        {
                            Description = "Внутренняя ошибка",
                            StatusCode = StatusCode.NotFind,
                        };
                    }

                    // Получение текущего склада, исходя из информации о перемещениях товара
                    var currentPositionResponse = await GetCurrentPositionProduct(ProductMove.Id);
                    Guid sourceWarehouseId;
                    if (currentPositionResponse.StatusCode == StatusCode.Ok)
                    {
                        // Если информация о перемещениях товара доступна, берем его текущий склад назначения
                        sourceWarehouseId = currentPositionResponse.Data.DestinationWarehouseId;
                    }
                    else
                    {
                        sourceWarehouseId = ProductMove.Warehouse.Id;
                    }
                    Guid DestinationWarehouseId = model.First().DestinationWarehouseId;
                    var movement = new ProductMovement()
                    {
                        ProductId = ProductMove.Id,
                        SourceWarehouseId = sourceWarehouseId,
                        DestinationWarehouseId = DestinationWarehouseId,
                        MovementDate = DateTime.Now,
                        Product = ProductMove
                    };
                    //Найдём по id Наименование склада на какой 
                    var FindName = await _warehouseRepository.GetAll()
                        .Where(x => x.Id == DestinationWarehouseId)
                        .Select(n => n.Name)
                        .FirstOrDefaultAsync();
                    //Найдем по id наименованиею склада, откуда 
                    var FindGetName = await _warehouseRepository.GetAll()
                        .Where(x=>x.Id == sourceWarehouseId)
                        .Select(x=>x.Name)
                        .FirstOrDefaultAsync();

                    await _transBaseRepository.Create(movement);
                    return new BaseResponse<IEnumerable<ProductMovement>>()
                    {
                        Description = $"{ProductMove.NameProduct} перемещён с \n {FindGetName} на {FindName}  ",
                        StatusCode = StatusCode.Ok
                    };
                }
                //В противном случае, когда там целая коллекция товаров

                var products = new List<Products>(); // Создаем список товаров для перемещения

                foreach (var prod in model)
                {
                    // Получаем товары по наименованию и инвентарному коду
                    var productList = await _productsRepository.GetAll()
                        .Include(w => w.Warehouse)
                        .Where(x =>x.Id == prod.Id)
                        .Where(x => x.UserId == null)
                        .ToListAsync();

                    products.AddRange(productList); // Добавляем товары в общий список
                }

                // Проверяем, что есть доступные товары для перемещения
                if (!products.Any())
                {
                    return new BaseResponse<IEnumerable<ProductMovement>>()
                    {
                        Description = "Нет доступных товаров на перемещение.",
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

                Guid destinationWarehouseId = model.First().DestinationWarehouseId; // Получаем id целевого склада

                // Выполняем операцию перемещения для указанного количества товаров
                foreach (var transfer in model)
                {
                    var transferCount = transfer.CountTransfer; // Количество перемещаемых товаров из текущей модели
                    var productName = transfer.NameProduct; // Наименование товара из текущей модели

                    // Получаем список товаров для текущей модели
                    var productsToTransfer = products.Where(p => p.NameProduct == productName).Take(transferCount);

                    foreach (var product in productsToTransfer)
                    {
                        // Получаем текущее местоположение товара
                        var currentPositionResponse = await GetCurrentPositionProduct(product.Id);
                        Guid sourceWarehouseId;

                        if (currentPositionResponse.StatusCode == StatusCode.Ok)
                        {
                            sourceWarehouseId = currentPositionResponse.Data.DestinationWarehouseId;
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
                            MovementDate = DateTime.Now
                        };
                        await _transBaseRepository.Create(movement);
                    }
                }

                // Составляем описание операции перемещения
                string destinationWarehouseName = ""; // Вставьте наименование целевого склада
                int lastDigit = totalTransferCount % 10;
                string description;
                if (totalTransferCount >= 11 && totalTransferCount <= 14)
                {
                    description = $"{totalTransferCount} товаров было перемещено с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }
                else if (lastDigit == 1)
                {
                    description = $"{totalTransferCount} товар был перемещён с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }
                else if (lastDigit >= 2 && lastDigit <= 4)
                {
                    description = $"{totalTransferCount} товара было перемещено с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }
                else
                {
                    description = $"{totalTransferCount} товаров было перемещено с {products[0].Warehouse.Name} на {destinationWarehouseName}";
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

        public async Task<BaseResponse<ProductMovement>> GetCurrentPositionProduct(Guid ProductId)
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
    }
}
