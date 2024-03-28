using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Domain.ViewModel.Warehouse;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Linq;

namespace HelpSystem.Service.Implementantions
{
    public class WarehouseService : IWarehouseService
    {
        private IBaseRepository<Warehouse> _warehouseRepository;
        private IBaseRepository<Products> _products;

        private IBaseRepository<User> _userRepository;
        private IBaseRepository<ProductMovement> _productMovementRepository;
        
        public WarehouseService(IBaseRepository<Warehouse> warehouse, IBaseRepository<Products> products,IBaseRepository<User> user, IBaseRepository<ProductMovement> productMovementRepository)
        {
            _warehouseRepository = warehouse;
            _products = products;
            _userRepository = user;
            _productMovementRepository = productMovementRepository;
        }
        public async Task<BaseResponse<Warehouse>> CreateWarehouse(WarehouseViewModel model)
        {
            try
            {
                var Warehouses = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == model.WarehouseName);
                if (Warehouses == null)
                {
                    var NewWarehouse = new Warehouse()
                    {
                        Name = model.WarehouseName
                    };
                    await _warehouseRepository.Create(NewWarehouse);
                    return new BaseResponse<Warehouse>()
                    {
                        Data = NewWarehouse,
                        Description = "Склад успешно создан",
                        StatusCode = StatusCode.Ok
                    };

                }
                else
                {
                    return new BaseResponse<Warehouse>()
                    {
                        Description = "Склад с таким наименованием уже существует.",
                        StatusCode = StatusCode.UnCreated
                    };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponse<Warehouse>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<Warehouse>> DeleteWarehouse(Guid id)
        {
            try
            {


                var DelWarehouse = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (DelWarehouse == null)
                {
                    return new BaseResponse<Warehouse>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Объект не найден"
                    };
                }

                await _warehouseRepository.Delete(DelWarehouse);
                return new BaseResponse<Warehouse>()
                {
                    Description = "Склад успешно удалён",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception e)
            {
                return new BaseResponse<Warehouse>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<WarehouseViewModel>>> GetAllWarehouse()
        {
            try
            {
                var AllWarehouse = await _warehouseRepository.GetAll()
                    .Select(x => new WarehouseViewModel()
                    {
                        Id = x.Id,
                        WarehouseName = x.Name,
                        TotalCountWarehouse = x.Products.Count
                    })
                    .ToListAsync();
                if (AllWarehouse.Count == 0)
                {
                    return new BaseResponse<IEnumerable<WarehouseViewModel>>()
                    {
                        Description = "Не удалось найти ни одного склада",
                        StatusCode = StatusCode.NotFind
                    };
                }

                //Если есть информация о том, что товар был перемещён на этот склад, делаем подсчёт и отображаем
                foreach (var warehouse in AllWarehouse)
                {
                    // Получаем количество записей, представляющих товары, прибывшие на этот склад
                    var incomingCount = await _productMovementRepository.GetAll()
                        .OrderByDescending(m=>m.MovementDate)
                        .Where(x => x.DestinationWarehouseId == warehouse.Id)
                        .CountAsync();

                    // Получаем количество записей, представляющих товары, ушедшие со склада
                    var outgoingCount = await _productMovementRepository.GetAll()
                        .OrderByDescending(m=>m.MovementDate)
                        .Where(x => x.SourceWarehouseId == warehouse.Id)
                        .CountAsync();

                    // Обновляем общее количество товаров на складе, добавляя количество пришедших записей и вычитая количество ушедших
                    warehouse.TotalCountWarehouse += incomingCount - outgoingCount;
                }

                return new BaseResponse<IEnumerable<WarehouseViewModel>>()
                {
                    Data = AllWarehouse,
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<WarehouseViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<WarehouseViewModel>> GetWarehouse(Guid id)
        {
            try
            {
                var Response = await _warehouseRepository.GetAll()
                    .Include(p=>p.Products)
                    .Select(x => new WarehouseViewModel()
                    {
                        Id = x.Id,
                        WarehouseName = x.Name,
                    }).FirstOrDefaultAsync(x => x.Id == id);
                if (Response != null)
                {
                    return new BaseResponse<WarehouseViewModel>()
                    {
                        Data = Response,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<WarehouseViewModel>()
                {
                    Description = $"Склад не найден",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<WarehouseViewModel>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Warehouse>> SaveWarehouse(WarehouseViewModel model)
        {
            try
            {
                var Response = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if (Response != null)
                {
                    if (Response.Name != model.WarehouseName)
                    {
                        var ExistWarehouse = await _warehouseRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.Name == model.WarehouseName);
                        if (ExistWarehouse != null)
                        {
                            return new BaseResponse<Warehouse>()
                            {
                                Description = "Склад с таким наименованием уже существует",
                                StatusCode = StatusCode.UnChanched
                            };
                        }
                        Response.Name = model.WarehouseName;
                        await _warehouseRepository.Update(Response);
                        return new BaseResponse<Warehouse>()
                        {
                            Data = Response,
                            Description = "Наименование склада успешно изменено",
                            StatusCode = StatusCode.Ok
                        };

                    }

                    return new BaseResponse<Warehouse>()
                    {
                        Description = "Не вижу никаких изменений",
                        StatusCode = StatusCode.UnChanched
                    };

                }

                return new BaseResponse<Warehouse>()
                {
                    Description = "Склад не найден",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Warehouse>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        //Получения списка всех товаров, находящихся на этом складе
        //TODO потом немного изменить логику так, чтобы тут были только те товары, которые на момент не перемещены
        public async Task<DataTableResponse> GetProductWarehouse(Guid id)
        {
            try
            {
                var Warehouse = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (Warehouse != null)
                {
                    // Получаем список всех товаров на выбранном складе
                    var productsOnWarehouse = await _products.GetAll()
                        .Where(p => p.Warehouse == Warehouse)
                        .ToListAsync();


                    if (!productsOnWarehouse.Any())
                    {

                        //Если нет товаров по накладной, но есть перемещённые, то делаем следущее

                        //Создаем список товаров, которые отправлены со склада по последним данным
                        var MovedProducts = await _productMovementRepository.GetAll()
                            .OrderByDescending(m => m.MovementDate)
                            .Where(x => x.SourceWarehouseId == Warehouse.Id)
                            .Select(x => x.Product)
                            .ToListAsync();
                        //Получаем текущие товары, которые поступили на склад по последним данным
                        var GetProducts = await _productMovementRepository.GetAll()
                            .OrderByDescending(x=>x.MovementDate)
                            .Where(x=>x.DestinationWarehouseId == Warehouse.Id)
                            .Select(x=>x.Product)
                            .ToListAsync();
                        var availableProducts = GetProducts.Except(MovedProducts);


                        // Группируем товары по их наименованию
                        var groupedProducts = availableProducts
                            .GroupBy(x => x.NameProduct)
                            .Select(group => new ProductinWarehouseViewModel()
                            {
                                NameProduct = group.Key,
                                CodeProduct = group.First().InventoryCode,
                                TotalCountWarehouse = group.Count(),
                                AvailableCount = group.Count(x => x.UserId == null)
                            })
                            .ToList();

                        // Возвращаем успешный ответ с данными
                        return new DataTableResponse()
                        {
                            Data = groupedProducts,

                        };
                    }
                    else
                    {
                        //Получаем список товаров, которые были отправлены с этого склада
                        var MovedProduct = await _productMovementRepository.GetAll()
                            .OrderByDescending(x => x.MovementDate)
                            .Where(x => x.SourceWarehouseId == Warehouse.Id)
                            .Select(m => m.Product)
                            .ToListAsync();
                        //Получаем список товаров, которые пришли на склад по последним данным
                        var GetProducts = await _productMovementRepository.GetAll()
                            .OrderByDescending(m => m.MovementDate)
                            .Where(x => x.DestinationWarehouseId == Warehouse.Id)
                            .Select(m => m.Product)
                            .ToListAsync();
                        //Исключаем из списка полученных товаров, отправленные по времени
                        var UnMovedProducts = productsOnWarehouse.Except(MovedProduct).Union(GetProducts);
                        //Добавляем к текущим, которые по накладной, те, что перемещены
                      
                        

                        //Группируем и также отправляем 
                        var GroupedProducts = UnMovedProducts
                            .GroupBy(x => x.NameProduct)
                            .Select(group => new ProductinWarehouseViewModel()
                            {
                                NameProduct = group.Key,
                                CodeProduct = group.First().InventoryCode,
                                TotalCountWarehouse = group.Count(),
                                AvailableCount = group.Count(x => x.UserId == null)
                            })
                            .ToList();
                        return new DataTableResponse()
                        {
                            Data = GroupedProducts
                        };

                    }
                }
                

                return new DataTableResponse()
                {
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponse()
                {
                    Data = null
                };
            }
        }

        public async Task<BaseResponse<Products>> BindWarehouseProduct(BindingProductWarehouse model)
        {
            try
            {
                //находим товары
                var Products = await _products.GetAll()
                    .Where(x => x.NameProduct == model.ProductName && x.InventoryCode == model.InventoryCode)
                    .Where(x => x.UserId == null)
                    .ToListAsync();
                //Товары, которые нужно привязать
                int CountWarehouseProduct = model.CountBinding;
                if (CountWarehouseProduct <= 0)
                {
                    return new BaseResponse<Products>()
                    {
                        Description = "Невозможно привязать товар с количеством, меньшим или равным 0",
                        StatusCode = StatusCode.NotFind
                    };
                }
                if (Products.Any())
                {

                    if (Products.Count < CountWarehouseProduct)
                    {
                        return new BaseResponse<Products>()
                        {
                            Description =
                                "Количество доступного товара, который необходимо привязать\n меньше требуемого.",
                            StatusCode = StatusCode.NotFind
                        };
                    }

                    var User = await _userRepository.GetAll()
                        .Include(p=>p.Profile)
                        .FirstOrDefaultAsync(x => x.Id == model.UserId);

                    if (User != null)
                    {
                        for (int i = 0; i < CountWarehouseProduct; i++)
                        {
                            Products[i].UserId = User.Id;
                            await _products.Update(Products[i]);
                        }
                            //Для более логичного окончания

                        string description;
                        int lastDigit = CountWarehouseProduct % 10;
                        if (CountWarehouseProduct >= 11 && CountWarehouseProduct <= 14) // Исключение для чисел 11-14
                        {
                            description = $"{CountWarehouseProduct} товаров было прикреплено к {User.Profile.Surname} {User.Profile.Name} {User.Profile.LastName}";
                        }
                        else if (lastDigit == 1)
                        {
                            description = $"{CountWarehouseProduct} товар был прикреплен к {User.Profile.Surname} {User.Profile.Name} {User.Profile.LastName}";
                        }
                        else if (lastDigit >= 2 && lastDigit <= 4)
                        {
                            description = $"{CountWarehouseProduct} товара было прикреплено к {User.Profile.Surname} {User.Profile.Name} {User.Profile.LastName}";
                        }
                        else
                        {
                            description = $"{CountWarehouseProduct} товаров было прикреплено к {User.Profile.Surname} {User.Profile.Name} {User.Profile.LastName}";
                        }



                        return new BaseResponse<Products>()
                        {


                            StatusCode = StatusCode.Ok,
                            Description = description
                        };
                    }
                }

                return new BaseResponse<Products>()
                {
                    Description = $"Нет доступного товара для привязки",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Products>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        //Тут мы получим все товары, не группируя их, тогда можно будет перемещать каждый товар по отдельности
        public async Task<BaseResponse<IEnumerable<TransferProductViewModel>>> GetProductsDetails(Guid WhId)
        {
            try
            {
                var Warehouse = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == WhId);
                if (Warehouse != null)
                {
                    //Получаем все товары на складе
                    var productsOnWarehouse = await _products.GetAll()
                        .Where(p => p.Warehouse == Warehouse && p.UserId == null)
                        .ToListAsync();
                    if (productsOnWarehouse.Any())
                    {
                        // Получаем последние записи перемещения для каждого товара на склад и из склада
                        var lastMovementsFromWarehouse = _productMovementRepository.GetAll()
          .Where(m => m.SourceWarehouseId == Warehouse.Id && m.Product.UserId == null)
          .GroupBy(m => m.ProductId)
          .Select(g => g.OrderByDescending(m => m.MovementDate).First().Product.Id)
          .ToList();

                        // Получаем список товаров, которые пришли на этот склад по последней записи
                        var lastMovementsToWarehouse = _productMovementRepository.GetAll()
                            .Where(m => m.DestinationWarehouseId == Warehouse.Id && m.Product.UserId == null)
                            .GroupBy(m => m.ProductId)
                            .Select(g => g.OrderByDescending(m => m.MovementDate).First().Product)
                            .ToList();

                        // Исключаем из основного списка товары, которые были перемещены с этого склада
                        var filteredProducts = productsOnWarehouse
                            .Where(product => !lastMovementsFromWarehouse.Contains(product.Id))
                            .Distinct()
                            .Union(lastMovementsToWarehouse.Select(product => new Products { Id = product.Id, NameProduct = product.NameProduct, InventoryCode = product.InventoryCode, Warehouse = Warehouse }))
                            .ToList();

                        // Получаем товары, которые пришли по накладной и не были перемещены
                        var incomingProducts = lastMovementsToWarehouse
                            .Where(productId => productsOnWarehouse.Any(p => p.Id == productId.Id))
                            .Select(productId => productsOnWarehouse.FirstOrDefault(p => p.Id == productId.Id))
                            .Distinct()
                            .ToList();
                            /*
                             * В связи с тем, что в отправленных и полученных товаров может быть одна и та же запись,
                             * мы просто исключим одно из другого, затем объединим
                             */
                            var ExceptionsProduct = incomingProducts.Except(lastMovementsToWarehouse);
                        // Объединяем товары, которые пришли по накладной и не были перемещены, с товарами, поступившими на склад по последней записи
                        var availableProducts = filteredProducts.Union(ExceptionsProduct).ToList();

                        // Получаем список всех складов, кроме текущего
                        var NotCurrentWarehouse = await _warehouseRepository.GetAll()
                            .Where(x => x.Id != WhId)
                            .ToListAsync();

                        // Формируем список деталей товара для аккордеона
                        var productDetails = availableProducts.Select(p => new TransferProductViewModel
                        {
                            Id = p.Id,
                            Name = p.NameProduct,
                            Code = p.InventoryCode,
                            Warehouses = NotCurrentWarehouse
                        }).ToList();

                        return new BaseResponse<IEnumerable<TransferProductViewModel>>
                        {
                            Data = productDetails,
                            StatusCode = StatusCode.Ok
                        };

                    }



                    else
                    {
                        // Получаем список товаров, которые пришли на этот склад
                        var incomingProducts = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .OrderByDescending(m=>m.MovementDate)
                            .Where(m => m.DestinationWarehouseId == Warehouse.Id && m.Product.UserId == null)
                            .ToListAsync();

                        // Получаем список товаров, которые уже были перемещены с текущего склада
                        var transferredProducts = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(m => m.SourceWarehouseId == Warehouse.Id)
                            .GroupBy(m => m.ProductId)
                            .Select(g => g.OrderByDescending(m => m.MovementDate).FirstOrDefault())
                            .ToListAsync();

                        // Формируем список доступных товаров, учитывая последнее перемещение
                        var availableProducts = incomingProducts.Where(p =>
                        {
                            var lastMovement = transferredProducts.FirstOrDefault(t => t.ProductId == p.ProductId);
                            return lastMovement == null || lastMovement.MovementDate < p.MovementDate;
                        });


                        // Получаем список складов, кроме текущего
                        var otherWarehouses = await _warehouseRepository.GetAll()
                            .Where(x => x.Id != WhId)
                            .ToListAsync();

                        // Формируем список деталей товара для аккордеона
                        var productDetails = availableProducts.Select(p => new TransferProductViewModel
                        {
                            Id = p.ProductId,
                            Name = p.Product.NameProduct,
                            Code = p.Product.InventoryCode,
                            Warehouses = otherWarehouses
                        }).ToList();

                        return new BaseResponse<IEnumerable<TransferProductViewModel>>
                        {
                            Data = productDetails,
                            StatusCode = StatusCode.Ok
                        };
                    }
                }

                return new BaseResponse<IEnumerable<TransferProductViewModel>>()
                {
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<TransferProductViewModel>>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"{ex.Message}"
                };
            }
        }




        /// <summary>
        /// Простой достаточно метод получения всех складов кроме текущего
        /// </summary>
        /// <param name="idGuid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IBaseResponse<IEnumerable<Warehouse>>> GetNotCurrentWH(Guid idGuid)
        {
            try
            {
                //Сразу же передадим список складов, кроме текущего.
                var NotCurrentWarehouse = await _warehouseRepository.GetAll()
                    .Where(x => x.Id != idGuid)
                    .ToListAsync();
                if (NotCurrentWarehouse.Any())
                {
                    return new BaseResponse<IEnumerable<Warehouse>>()
                    {
                        Data = NotCurrentWarehouse,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<IEnumerable<Warehouse>>()
                {
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Warehouse>>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"{ex.Message}"
                };
            }
        }
    }
}
