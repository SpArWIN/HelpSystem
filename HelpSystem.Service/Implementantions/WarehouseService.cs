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
        public async Task<DataTableResponse> GetProductWarehouse(Guid id)
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
                            .Include(p=>p.Product)
                            .Where(x => x.DestinationWarehouseId == warehouse.Id)
                            .OrderByDescending(x => x.MovementDate)
                            .ToListAsync();

                        // Получаем записи о перемещениях товаров, которые ушли со склада
                        var outgoingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.SourceWarehouseId == warehouse.Id)
                            .OrderByDescending(x => x.MovementDate)
                            .ToListAsync();

                        // Получаем только что пришедшие товары
                        var latestIncomingProducts = incomingMovements.Select(x => x.Product).ToList();

                        // Получаем только что ушедшие товары
                        var latestOutgoingProducts = outgoingMovements.Select(x => x.Product).ToList();

                        // Обновляем список товаров на складе
                        foreach (var incomingProduct in latestIncomingProducts)
                        {
                            if (!latestOutgoingProducts.Any(x => x.Id == incomingProduct.Id))
                            {
                                productsOnWarehouse.Add(incomingProduct);
                            }
                        }

                        foreach (var outgoingProduct in latestOutgoingProducts)
                        {
                            if (!latestIncomingProducts.Any(x => x.Id == outgoingProduct.Id))
                            {
                                productsOnWarehouse.Remove(outgoingProduct);
                            }
                        }

                        // Группируем товары по их наименованию и создаем список ProductinWarehouseViewModel
                        var GroupedProducts = productsOnWarehouse
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

                    var GetMovedProducts = await _productMovementRepository.GetAll()
                        .Where(x => x.DestinationWarehouseId == warehouse.Id)
                        .Select(x => x.Product)
                        .ToListAsync();

                    var MovedProductsNull = await _productMovementRepository.GetAll()
                        .OrderByDescending(x => x.MovementDate)
                        .Where(x => x.SourceWarehouseId == warehouse.Id)
                        .Select(x => x.Product)
                        .ToListAsync();

                    List<Products> SumList = new List<Products>();
                    if (GetMovedProducts.Any())
                    {
                        SumList = GetMovedProducts.Except(MovedProductsNull).ToList();
                    }
                    else
                    {
                        SumList = SumList.Except(MovedProductsNull).ToList();
                    }

                    var groupedProducts = SumList
                        .GroupBy(x => x.NameProduct)
                        .Select(group => new ProductinWarehouseViewModel()
                        {
                            NameProduct = group.Key,
                            CodeProduct = group.First().InventoryCode,
                            TotalCountWarehouse = group.Count(),
                            AvailableCount = group.Count(x => x.UserId == null)
                        }).ToList();
                    return new DataTableResponse()
                    {
                        Data = groupedProducts
                    };
                    // Группируем товары по наименованию
                    //var groupedProducts = allProducts
                    //    .GroupBy(x => x.NameProduct)
                    //    .Select(group => new ProductinWarehouseViewModel()
                    //    {

                    //        NameProduct = group.Key,
                    //        CodeProduct = group.First().InventoryCode,
                    //        TotalCountWarehouse = group.Count(),
                    //        AvailableCount = group.Count(x => x.UserId == null)
                    //    })
                    //    .ToList();



                }

                return new DataTableResponse()
                {
                    Data = null
                };
            }


            catch (Exception e)
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
                // Получаем список всех товаров на конкретном складе, которые пришли по накладной
                var availableProducts = await _products.GetAll()
                    .Where(x => x.NameProduct == model.ProductName &&
                                x.InventoryCode == model.InventoryCode &&
                                x.UserId == null &&
                                x.Warehouse.Id == model.WarehouseId).ToListAsync();

                List<ProductMovement>? movementRecords;
                List<Products> availableProductsOnWarehouse;
                if (!availableProducts.Any())
                {
                    // Получаем все записи о перемещении товаров на этот склад
                    movementRecords = await _productMovementRepository.GetAll()
                        .Include(p => p.Product)
                        .Where(x => x.DestinationWarehouseId == model.WarehouseId)
                        .OrderByDescending(m => m.MovementDate)
                        .ToListAsync();

                    if (!movementRecords.Any())
                    {
                        return new BaseResponse<Products>()
                        {
                            Description = "Нет доступного товара на складе.",
                            StatusCode = StatusCode.NotFind
                        };
                    }

                    // Получаем список товаров, которые ушли со склада
                    var outgoingProducts = movementRecords
                        .Where(m => m.SourceWarehouseId == model.WarehouseId && m.MovementDate <= DateTime.Now)
                        .Select(m => m.Product)
                        .ToList();

                    // Получаем список товаров, которые пришли на склад
                    var incomingProducts = movementRecords
                        .Where(m => m.DestinationWarehouseId == model.WarehouseId && m.MovementDate <= DateTime.Now && m.Product.UserId == null)
                        .Select(m => m.Product)
                        .ToList();

                    // Убираем из списка пришедших товаров те, которые ушли
                    foreach (var outgoingProduct in outgoingProducts)
                    {
                        incomingProducts.RemoveAll(p => p.Id == outgoingProduct.Id);
                    }

                    int CountTakeWarehouse = model.CountBinding;
                    // Прикрепляем к пользователю оставшийся перемещенный товар
                    // Проверяем его наличие на складе
                    if (CountTakeWarehouse <= 0)
                    {
                        return new BaseResponse<Products>()
                        {
                            Description = "Невозможно привязать товар с количеством, меньшим или равным 0",
                            StatusCode = StatusCode.NotFind
                        };
                    }

                    var Usver = await _userRepository.GetAll()
                        .Include(p => p.Profile)
                        .FirstOrDefaultAsync(x => x.Id == model.UserId);
                    if (Usver != null)
                    {
                        if (incomingProducts.Count < CountTakeWarehouse)
                        {
                            return new BaseResponse<Products>()
                            {
                                StatusCode = StatusCode.NotFind,
                                Description = $"Нет доступного товара для привязки"
                            };

                        }

                        int Counts = 0;
                        foreach (var incom in incomingProducts)
                        {
                          
                            if (Counts == CountTakeWarehouse)
                            {
                                break;    // Если мы достигли нужного количества товаров для привязки, выходим из цикла
                            }


                            incom.UserId= Usver.Id;
                            await _products.Update(incom);
                            Counts++;

                        }

                        string description;
                        int lastDigit = CountTakeWarehouse % 10;
                        if (CountTakeWarehouse >= 11 && CountTakeWarehouse <= 14) // Исключение для чисел 11-14
                        {
                            description =
                                $"{CountTakeWarehouse} товаров было прикреплено к {Usver.Profile.LastName} {Usver.Profile.Name} {Usver.Profile.Surname}";
                        }
                        else if (lastDigit == 1)
                        {
                            description =
                                $"{CountTakeWarehouse} товар был прикреплен к {Usver.Profile.LastName} {Usver.Profile.Name} {Usver.Profile.Surname}";
                        }
                        else if (lastDigit >= 2 && lastDigit <= 4)
                        {
                            description =
                                $"{CountTakeWarehouse} товара было прикреплено к {Usver.Profile.LastName} {Usver.Profile.Name} {Usver.Profile.Surname}";
                        }
                        else
                        {
                            description =
                                $"{CountTakeWarehouse} товаров было прикреплено к {Usver.Profile.LastName} {Usver.Profile.Name} {Usver.Profile.Surname}";
                        }


                        return new BaseResponse<Products>()
                        {

                            StatusCode = StatusCode.Ok,
                            Description = description
                        };





                    }
                  
                    return new BaseResponse<Products>()
                    {
                            Description = $"Пользователь не найден.",
                            StatusCode = StatusCode.NotFind
                    };
                    
                    
                }
                else
                {




                    // Получаем все записи о перемещении товаров на или с этого склада
                    movementRecords = await _productMovementRepository.GetAll()
                        .Where(x => x.SourceWarehouseId == model.WarehouseId ||
                                    x.DestinationWarehouseId == model.WarehouseId)
                        .OrderByDescending(m => m.MovementDate)
                        .ToListAsync();

                    // Фильтруем товары, которые уже были перемещены с/на склад
                    var movedProductIds = movementRecords.Select(m => m.ProductId);

                    // Фильтруем доступные товары, которые не были перемещены

                    availableProductsOnWarehouse = availableProducts
                        .Where(p => !movedProductIds.Contains(p.Id))
                        .ToList();

                    // Если на складе нет нужного количества товаров, 
                    // добавляем перемещенные товары
                    if (availableProductsOnWarehouse.Count() < model.CountBinding)
                    {
                        var movedProductsCount = movementRecords
                            .Count(m => m.DestinationWarehouseId == model.WarehouseId &&
                                        m.MovementDate <= DateTime.Now);

                        // Доступное количество = основные + перемещенные
                        if (availableProductsOnWarehouse.Count() + movedProductsCount >= model.CountBinding)
                        {
                            availableProductsOnWarehouse.AddRange(movementRecords
                                .Where(m => m.DestinationWarehouseId == model.WarehouseId &&
                                            m.MovementDate <= DateTime.Now)
                                .Select(m => m.Product)
                                .ToList());
                        }
                    }

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

                    if (availableProductsOnWarehouse.Count() < CountWarehouseProduct)
                    {
                        return new BaseResponse<Products>()
                        {
                            Description =
                                "Количество доступного товара, который необходимо привязать\n меньше требуемого.",
                            StatusCode = StatusCode.NotFind
                        };
                    }


                    var User = await _userRepository.GetAll()
                        .Include(p => p.Profile)
                        .FirstOrDefaultAsync(x => x.Id == model.UserId);

                    if (User != null)
                    {
                        int count = 0;
                        foreach (var product in availableProductsOnWarehouse)
                        {
                            if (count == CountWarehouseProduct)
                            {
                                break; // Если мы достигли нужного количества товаров для привязки, выходим из цикла
                            }

                            product.UserId = User.Id;
                            await _products.Update(product);
                            count++;
                        }
                        //Для более логичного окончания

                        string description;
                        int lastDigit = CountWarehouseProduct % 10;
                        if (CountWarehouseProduct >= 11 && CountWarehouseProduct <= 14) // Исключение для чисел 11-14
                        {
                            description =
                                $"{CountWarehouseProduct} товаров было прикреплено к {User.Profile.LastName} {User.Profile.Name} {User.Profile.Surname}";
                        }
                        else if (lastDigit == 1)
                        {
                            description =
                                $"{CountWarehouseProduct} товар был прикреплен к {User.Profile.LastName} {User.Profile.Name} {User.Profile.Surname}";
                        }
                        else if (lastDigit >= 2 && lastDigit <= 4)
                        {
                            description =
                                $"{CountWarehouseProduct} товара было прикреплено к {User.Profile.LastName} {User.Profile.Name} {User.Profile.Surname}";
                        }
                        else
                        {
                            description =
                                $"{CountWarehouseProduct} товаров было прикреплено к {User.Profile.LastName} {User.Profile.Name} {User.Profile.Surname}";
                        }


                        return new BaseResponse<Products>()
                        {

                            StatusCode = StatusCode.Ok,
                            Description = description
                        };

                    }

                    return new BaseResponse<Products>()
                    {
                        Description = $"Пользователь не найден.",
                        StatusCode = StatusCode.NotFind
                    };
                }
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
