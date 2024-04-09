using System.ComponentModel.DataAnnotations;
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
using System.Collections.Generic;

namespace HelpSystem.Service.Implementantions
{
    public class WarehouseService : IWarehouseService
    {
        private IBaseRepository<Warehouse> _warehouseRepository;
        private IBaseRepository<Products> _products;

        private IBaseRepository<User> _userRepository;
        private IBaseRepository<ProductMovement> _productMovementRepository;

        public WarehouseService(IBaseRepository<Warehouse> warehouse, IBaseRepository<Products> products,
            IBaseRepository<User> user, IBaseRepository<ProductMovement> productMovementRepository)
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
                        TotalCountWarehouse = x.Products.Count()
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
                        .OrderByDescending(m => m.MovementDate)
                        .Where(x => x.DestinationWarehouseId == warehouse.Id)
                        .CountAsync();

                    // Получаем количество записей, представляющих товары, ушедшие со склада
                    var outgoingCount = await _productMovementRepository.GetAll()
                        .OrderByDescending(m => m.MovementDate)
                        .Where(x => x.SourceWarehouseId == warehouse.Id)
                        .CountAsync();
                    // Добавляем уже имеющееся количество товаров на складе
                    var existingCount = warehouse.TotalCountWarehouse;

                    // Обновляем общее количество товаров на складе, добавляя количество пришедших записей и вычитая количество ушедших
                    warehouse.TotalCountWarehouse = (existingCount + incomingCount) - outgoingCount;
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
                    .Include(p => p.Products)
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

        //Получения списка всех товаров, находящихся на этом складе, а также перемещенных товаров на склад
        public async Task<BaseResponse<IEnumerable<ProductinWarehouseViewModel>>> GetProductWarehouse(Guid id)
        {
            try
            {
                var warehouse = await _warehouseRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (warehouse != null)
                {
                    //Пора это заканчивать, завтра прежде чем садиться, распиши алгоритм
                    var productsOnWarehouse = await _products.GetAll()
                        .Where(x => x.Warehouse == warehouse)
                        .ToListAsync();

                        

                    if (productsOnWarehouse.Any())
                    {
                        var incomingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.DestinationWarehouseId == warehouse.Id)
                            .OrderByDescending(m=>m.MovementDate)
                            .ToListAsync();

                        // Получаем записи о перемещениях товаров, которые ушли со склада
                        var outgoingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.SourceWarehouseId == warehouse.Id)
                            .OrderByDescending(m=>m.MovementDate)
                            .ToListAsync();

                        foreach (var incomingMovement in incomingMovements)
                        {
                            if (!outgoingMovements.Any(x => x.Product.Id == incomingMovement.Product.Id && x.MovementDate > incomingMovement.MovementDate))
                            {
                                productsOnWarehouse.Add(incomingMovement.Product);
                            }
                        }

                        // Перебираем только что ушедшие товары и удаляем их со склада, если последнее перемещение из склада было после последнего перемещения на него
                        foreach (var outgoingMovement in outgoingMovements)
                        {
                            if (!incomingMovements.Any(x => x.Product.Id == outgoingMovement.Product.Id && x.MovementDate < outgoingMovement.MovementDate))
                            {
                                productsOnWarehouse.Remove(outgoingMovement.Product);
                            }
                        }



                        // Группируем товары по их наименованию и создаем список ProductinWarehouseViewModel

                        var GroupedProducts = productsOnWarehouse
                            .GroupBy(x => x.NameProduct)
                            .Select(group => new ProductinWarehouseViewModel()
                            {
                              TotalCountWarehouse = group.Count(),
                              NameProduct = group.Key,
                              WhProducts = group.Select(p=>new ProductListWarehouse()
                              {
                                  Id = p.Id,
                                  NameProduct = p.NameProduct,
                                  CodeProduct = p.InventoryCode,
                                  AvailableCount = (p.UserId == null) ? 1 : 0
                              }).ToList(),
                            }).ToList();

                            


                        //var Products = productsOnWarehouse
                        //    .Select(group=>new ProductinWarehouseViewModel()
                        //    {
                        //        NameProduct = group.NameProduct,
                        //        CodeProduct = group.InventoryCode,
                        //        AvailableCount = 
                        //    })

                        return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                        {
                            Data = GroupedProducts
                        };


                    }
                    //Товары которые пришли
                    var GetProduct = await _productMovementRepository.GetAll()
                        .Include(p => p.Product)
                        .Where(x => x.DestinationWarehouseId == warehouse.Id)
                        .OrderByDescending(x => x.MovementDate)
                        .ToListAsync();
                    //Товары, которые ушли со склада
                    List<Products> SumList = new List<Products>();
                    var MovedProductsNull = await _productMovementRepository.GetAll()
                        .Include(p => p.Product)
                        .Where(x => x.SourceWarehouseId == warehouse.Id)
                        .OrderByDescending(x => x.MovementDate)
                        .ToListAsync();

                    // Получаем только что пришедшие товары
                    var GetIncoimingProducts = GetProduct.Select(x => x.Product).ToList();

                    // Получаем только что ушедшие товары
                    var MovedProducts = MovedProductsNull.Select(x => x.Product).ToList();

                    foreach (var product in GetIncoimingProducts)
                    {
                        if (GetIncoimingProducts.Any(x => x.Id == product.Id))
                        {
                            SumList.Add(product);
                        }
                    }

                    foreach (var OutProduct in MovedProducts)
                    {
                        if (MovedProducts.Any(x => x.Id == OutProduct.Id))
                        {
                            SumList.Remove(OutProduct);
                        }
                    }


                    var groupedProducts = SumList
                        .GroupBy(x => x.NameProduct)
                        .Select(group => new ProductinWarehouseViewModel()
                        {
                            NameProduct = group.Key,
                            TotalCountWarehouse = group.Count(),
                            WhProducts = group.Select(p => new ProductListWarehouse()
                            {
                                Id = p.Id,
                                NameProduct = p.NameProduct,
                                CodeProduct = p.InventoryCode,
                                AvailableCount = group.Count(item => item.UserId == null)
                            }).ToList(),
                        }).ToList();
                    return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                    {
                        Data = groupedProducts
                    };






                }

                return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                {
                    Data = null
                };
            }


            catch (Exception e)
            {
                return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                {
                    Data = null
                };
            }
        }





        public async Task<BaseResponse<Products>> BindWarehouseProduct(BindingProductWarehouse model)
        {
            try
            {


                //Получаем этот товар, который хотим закрепить
                // Проверяем наличие товара на текущем складе и его привязку
                var availableProduct = await _products.GetAll()
                    .Where(x => x.Id == model.ProductId && x.Warehouse.Id == model.WarehouseId && x.UserId == null)
                    .FirstOrDefaultAsync();

                if (availableProduct != null)
                {
                    // Проверяем наличие пользователя
                    var user = await _userRepository.GetAll()
                        .Include(u => u.Profile)
                        .FirstOrDefaultAsync(u => u.Id == model.UserId);

                    if (user == null)
                    {
                        return new BaseResponse<Products>()
                        {
                            Description = $"Пользователь не найден.",
                            StatusCode = StatusCode.NotFind
                        };
                    }

                    // Привязываем товар к пользователю
                    availableProduct.UserId = user.Id;
                    await _products.Update(availableProduct);

                    string description = $"Товар {availableProduct.NameProduct} был прикреплен к {user.Profile.LastName} {user.Profile.Name} {user.Profile.Surname}";

                    return new BaseResponse<Products>()
                    {
                        StatusCode = StatusCode.Ok,
                        Description = description,
                        Data = availableProduct
                    };
                }

                // Если товар не найден на текущем складе, проверяем последнее перемещение
                var lastMovement = await _productMovementRepository.GetAll()
                    .OrderByDescending(m => m.MovementDate)
                    .FirstOrDefaultAsync(m => m.Product.Id == model.ProductId);

                if (lastMovement != null && lastMovement.SourceWarehouseId != model.WarehouseId)
                {
                    var user = await _userRepository.GetAll()
                        .Include(u => u.Profile)
                        .FirstOrDefaultAsync(u => u.Id == model.UserId);
                    if (user == null)
                    {
                        return new BaseResponse<Products>()
                        {
                            Description = $"Пользователь не найден.",
                            StatusCode = StatusCode.NotFind
                        };
                    }
                    var Product = lastMovement.Product;
                    if (Product.UserId == null)
                    {
                        Product.UserId = user.Id;
                        await _products.Update(Product);
                    }
                    else
                    {
                        return new BaseResponse<Products>()
                        {
                            Description = $"Товар недоступен для привязки",
                            StatusCode = StatusCode.UnChanched
                        };
                    }
                }

                return new BaseResponse<Products>()
                {
                    Description = "Товар не доступен на указанном складе для привязки.",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Products>()
                {
                    Description = $"Ошибка при привязке товара: {ex.Message}",
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
                        var incomingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.DestinationWarehouseId == WhId)
                            .OrderByDescending(m => m.MovementDate)
                            .ToListAsync();

                        var outgoingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.SourceWarehouseId == WhId)
                            .OrderByDescending(m => m.MovementDate)
                            .ToListAsync();

                        // Получаем список всех складов, кроме текущего
                        var NotCurrentWarehouse = await _warehouseRepository.GetAll()
                            .Where(x => x.Id != WhId)
                            .ToListAsync();
                        foreach (var incomingMovement in incomingMovements)
                        {
                            if (!outgoingMovements.Any(x => x.Product.Id == incomingMovement.Product.Id && x.MovementDate > incomingMovement.MovementDate))
                            {
                                productsOnWarehouse.Add(incomingMovement.Product);
                            }
                        }

                        // Перебираем только что ушедшие товары и удаляем их со склада, если последнее перемещение из склада было после последнего перемещения на него
                        foreach (var outgoingMovement in outgoingMovements)
                        {
                            if (!incomingMovements.Any(x => x.Product.Id == outgoingMovement.Product.Id && x.MovementDate < outgoingMovement.MovementDate))
                            {
                                productsOnWarehouse.Remove(outgoingMovement.Product);
                            }
                        }



                        // Формируем список деталей товара для аккордеона
                        var productDetails = productsOnWarehouse
                            .Select(p => new TransferProductViewModel
                                
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
