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

        public async Task<BaseResponse<ProductMovement>> AddTransfer(TransferViewModel model)
        {
            try
            {

                // Получаем товары по наименованию и инвентарному коду
                var products = await _productsRepository.GetAll()
                    .Include(w => w.Warehouse)
                    .Where(x => x.NameProduct == model.NameProduct && x.InventoryCode == model.CodeProduct)
                    .Where(x => x.UserId == null)
                    .ToListAsync();

                if (!products.Any())
                {
                    return new BaseResponse<ProductMovement>()
                    {
                        Description = "Нет доступных товаров на перемещение.",
                        StatusCode = StatusCode.Ok,
                    };
                }

                // Проверяем, что запрошенное количество товаров доступно для перемещения
                int availableCount = products.Count;
                if (availableCount < model.CountTransfer)
                {
                    return new BaseResponse<ProductMovement>()
                    {
                        Description = "Запрошено больше количество перемещаемых товаров, чем доступно",
                        StatusCode = StatusCode.UnMove
                    };
                }

                Guid destinationWarehouseId = model.DestinationWarehouseId;

                // Выполняем операцию перемещения для указанного количества товаров
                foreach (var product in products.Take(model.CountTransfer))
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

                // Составляем описание операции перемещения
                string destinationWarehouseName = ""; // Вставьте наименование целевого склада
                int lastDigit = model.CountTransfer % 10;
                string description;
                if (model.CountTransfer >= 11 && model.CountTransfer <= 14)
                {
                    description = $"{model.CountTransfer} товаров было перемещено с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }
                else if (lastDigit == 1)
                {
                    description = $"{model.CountTransfer} товар был перемещён с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }
                else if (lastDigit >= 2 && lastDigit <= 4)
                {
                    description = $"{model.CountTransfer} товара было перемещено с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }
                else
                {
                    description = $"{model.CountTransfer} товаров было перемещено с {products[0].Warehouse.Name} на {destinationWarehouseName}";
                }

                return new BaseResponse<ProductMovement>()
                {
                    StatusCode = StatusCode.Ok,
                    Description = description
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductMovement>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.Ok
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
                        Description = $"Товар находится на складе {WhName}",
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
