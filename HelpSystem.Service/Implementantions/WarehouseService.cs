using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Domain.ViewModel.Warehouse;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{

    public class WarehouseService : IWarehouseService
    {
        private IBaseRepository<Warehouse> _warehouseRepository;
        private IBaseRepository<Products> _products;

        private IBaseRepository<User> _userRepository;
        private IBaseRepository<ProductMovement> _productMovementRepository;
        private IBaseRepository<Invoice> _invoiceRepository;
        public WarehouseService(IBaseRepository<Invoice> inv, IBaseRepository<Warehouse> warehouse, IBaseRepository<Products> products,
            IBaseRepository<User> user, IBaseRepository<ProductMovement> productMovementRepository)
        {
            _warehouseRepository = warehouse;
            _products = products;
            _userRepository = user;
            _productMovementRepository = productMovementRepository;
            _invoiceRepository = inv;
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
                        Name = model.WarehouseName,
                        IsFreeZing = false
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

        public async Task<BaseResponse<Warehouse>> FreezeWarehouse(Guid id)
        {
            try
            {


                var FreeseHouse = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (FreeseHouse == null)
                {
                    return new BaseResponse<Warehouse>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Объект не найден"
                    };
                }


                FreeseHouse.IsFreeZing = true;
                await _warehouseRepository.Update(FreeseHouse);
                return new BaseResponse<Warehouse>()
                {
                    Description = $"Склад {FreeseHouse.Name} заморожен\nВсе последующие операции приостановлены",
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
        public async Task<BaseResponse<Warehouse>> UNFreezeWarehouse(Guid id)
        {
            try
            {
                var UnFreezeWarehouse = await _warehouseRepository.GetAll()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
                if (UnFreezeWarehouse == null)
                {
                    return new BaseResponse<Warehouse>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Склад не найден"
                    };
                }
                UnFreezeWarehouse.IsFreeZing = false;
                await _warehouseRepository.Update(UnFreezeWarehouse);
                return new BaseResponse<Warehouse>()
                {
                    StatusCode = StatusCode.Ok,
                    Description = $"Склад {UnFreezeWarehouse.Name} разморожен. \nВсе последующие операции возобновлены."

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
        public async Task<BaseResponse<IEnumerable<WarehouseViewModel>>> GetAllWarehouse()
        {
            try
            {
                Dictionary<int, int> productCounts = new Dictionary<int, int>();
                var AllWarehouse = await _warehouseRepository.GetAll()
                    .Select(x => new WarehouseViewModel()
                    {
                        Id = x.Id,
                        WarehouseName = x.Name,
                        TotalCountWarehouse = x.Products.Count(),
                        isFreesing = x.IsFreeZing,
                        isService = x.IsService
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
                    // Получаем количество уникальных товаров, прибывших на этот склад

                    var incomingMovements = await _productMovementRepository.GetAll()
                           .Include(p => p.Product)
                           .Where(x => x.DestinationWarehouseId == warehouse.Id)
                           .ToListAsync();

                    // Получаем записи о перемещениях товаров, которые ушли со склада
                    var outgoingMovements = await _productMovementRepository.GetAll()
                        .Include(p => p.Product)
                        .Where(x => x.SourceWarehouseId == warehouse.Id)

                        .ToListAsync();
                    int TotalCount = 0;

                    foreach (var movement in incomingMovements)
                    {
                        if (!outgoingMovements.Any(x => x.ProductId == movement.ProductId && x.MovementDate > movement.MovementDate))
                        {
                            TotalCount++;
                        }
                    }

                    foreach (var movement in outgoingMovements)
                    {
                        if (!incomingMovements.Any(x => x.ProductId == movement.ProductId && x.MovementDate < movement.MovementDate))
                        {
                            TotalCount--;
                        }
                    }


                    // Добавляем уже имеющееся количество товаров на складе
                    var existingCount = warehouse.TotalCountWarehouse;

                    // Обновляем общее количество товаров на складе, добавляя количество пришедших товаров и вычитая количество ушедших
                    warehouse.TotalCountWarehouse = existingCount + TotalCount;
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
                        isService = x.IsService
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

                    //Косяк где-то тут

                    if (productsOnWarehouse.Any())
                    {
                        var incomingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.DestinationWarehouseId == warehouse.Id)
                            .ToListAsync();

                        // Получаем записи о перемещениях товаров, которые ушли со склада
                        var outgoingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.SourceWarehouseId == warehouse.Id)
                            .ToListAsync();

                        foreach (var movement in outgoingMovements)
                        {
                            // Проверяем, является ли товар "возвращенным" и его перемещение было позже последнего перемещения на склад

                            var existingProduct = productsOnWarehouse.FirstOrDefault(p => p.Id == movement.Product.Id);
                            if (existingProduct != null)
                            {
                                productsOnWarehouse.Remove(existingProduct);
                            }

                            if (incomingMovements.Any(x => x.Product.Id == movement.Product.Id && x.MovementDate > movement.MovementDate))
                            {

                                productsOnWarehouse.Add(movement.Product);
                            }
                        }

                        // Добавляем товары, которые приходят на текущий склад
                        foreach (var movement in incomingMovements)
                        {
                            // Проверяем, является ли товар "новым" или его последнее перемещение было ранее, чем последнее перемещение со склада
                            if (!outgoingMovements.Any(x => x.Product.Id == movement.Product.Id && x.MovementDate > movement.MovementDate))
                            {
                                // Проверяем, содержится ли товар уже на складе
                                var existingProduct = productsOnWarehouse.FirstOrDefault(p => p.Id == movement.Product.Id);
                                if (existingProduct == null)
                                {
                                    productsOnWarehouse.Add(movement.Product);
                                }
                            }
                        }





                        // Группируем товары по их наименованию и создаем список ProductinWarehouseViewModel

                        var GroupedProducts = productsOnWarehouse
                            .GroupBy(x => x.NameProduct)
                            .Select(group => new ProductinWarehouseViewModel()
                            {
                                TotalCountWarehouse = group.Count(),
                                NameProduct = group.Key,
                                WhProducts = group.Select(p => new ProductListWarehouse()
                                {
                                    Id = p.Id,
                                    NameProduct = p.NameProduct,
                                    CodeProduct = p.InventoryCode,
                                    AvailableCount = (p.UserId == null) ? 1 : 0
                                })
                              .ToList(),
                            }).ToList();





                        return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                        {
                            Data = GroupedProducts,
                            StatusCode = StatusCode.Ok
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
                                AvailableCount = (p.UserId == null) ? 1 : 0
                            }).ToList(),
                        }).ToList();
                    return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                    {
                        Data = groupedProducts,
                        StatusCode = StatusCode.Ok
                    };






                }

                return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                {
                    Data = null,
                    StatusCode = StatusCode.NotFind
                 
                };
            }


            catch (Exception e)
            {
                return new BaseResponse<IEnumerable<ProductinWarehouseViewModel>>()
                {
                    Data = null,
                    StatusCode = StatusCode.InternalServerError
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
                    availableProduct.Comments = model.Comments;
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
                    .Include(p => p.Product)
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
                        Product.Comments = model.Comments;
                        await _products.Update(Product);
                        return new BaseResponse<Products>()
                        {
                            Description =
                                $"{lastMovement.Product.NameProduct} {lastMovement.Product.InventoryCode} был прикреплён к {user.Profile.LastName} {user.Profile.Name} {user.Profile.Surname}",
                            StatusCode = StatusCode.Ok,
                            Data = Product
                        };
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
                            .Where(x => x.DestinationWarehouseId == WhId && x.Product.UserId == null)
                         
                            .ToListAsync();


                        var outgoingMovements = await _productMovementRepository.GetAll()
                            .Include(p => p.Product)
                            .Where(x => x.SourceWarehouseId == WhId && x.Product.UserId == null)
                            .ToListAsync();

                        // Получаем список всех складов, кроме текущего
                        var NotCurrentWarehouse = await _warehouseRepository.GetAll()
                            .Where(x => x.Id != WhId && !x.IsService)
                          .ToListAsync();

                        foreach (var movement in outgoingMovements)
                        {
                            // Проверяем, является ли товар "возвращенным" и его перемещение было позже последнего перемещения на склад

                            var existingProduct = productsOnWarehouse.FirstOrDefault(p => p.Id == movement.Product.Id);

                            if (existingProduct != null)
                            {
                                productsOnWarehouse.Remove(existingProduct);
                            }

                            if (incomingMovements.Any(x => x.Product.Id == movement.Product.Id && x.MovementDate > movement.MovementDate))
                            {

                                productsOnWarehouse.Add(movement.Product);
                            }
                        }

                        // Добавляем товары, которые приходят на текущий склад
                        foreach (var movement in incomingMovements)
                        {
                            // Проверяем, является ли товар "новым" или его последнее перемещение было ранее, чем последнее перемещение со склада
                            if (!outgoingMovements.Any(x => x.Product.Id == movement.Product.Id && x.MovementDate > movement.MovementDate))
                            {
                                // Проверяем, содержится ли товар уже на складе
                                var existingProduct = productsOnWarehouse.FirstOrDefault(p => p.Id == movement.Product.Id);
                                if (existingProduct == null)
                                {
                                    productsOnWarehouse.Add(movement.Product);
                                }
                            }
                        }



                     



                        // Формируем список деталей товара для аккордеона
                        var productDetails = productsOnWarehouse
                            .OrderBy(p => p.Id)
                            
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
                            .OrderByDescending(m => m.MovementDate)
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
                    .Where(x => x.Id != idGuid && x.IsFreeZing == false)
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

        public async Task<BaseResponse<Guid>> GetDetWarehouse()
        {
            try
            {
                var DetamingWh = await _warehouseRepository.GetAll()
                    .Where(x => x.IsService == true)
                   .Select(x => x.Id).FirstOrDefaultAsync();

                if (DetamingWh == Guid.Empty)
                {
                    return new BaseResponse<Guid>()
                    {
                        Data = Guid.Empty,
                        StatusCode = StatusCode.NotFind
                    };
                }
                return new BaseResponse<Guid>()
                {
                    Data = DetamingWh,
                    StatusCode = StatusCode.Ok,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Guid>()
                {
                    StatusCode = StatusCode.InternalServerError,
                };

            }
        }
        //Метод получения списанных товаров на складе утилизации
        public async Task<IBaseResponse<IEnumerable<ProductDebitingWarehouseViewModel>>> GetDebitingProduct()
        {
            //Так как мы точно знаем, что это будет склад утилизации, потому что только он пойдёт по этому маршруту.
            //Проверять смысла на сервис просто нет :)

            try
            {
                var products = await _products.GetAll()
                    .Include(w => w.Warehouse)
                 .Where(p => p.TimeDebbiting != null) // Фильтр по складу и времени списания
                 .ToListAsync();
                var result = new List<ProductDebitingWarehouseViewModel>();

                var ProductDebiting = new List<ProductDebitingViewModel>();
                if (products.Any())
                {

                    result = products
               .GroupBy(x => x.NameProduct)
               .Select(group => new ProductDebitingWarehouseViewModel()
                    {
                   ProductName = group.Key,
                   TotalCount = group.Count(),
                   Whproduct = group.Select(x =>
                    {
                        var invoice = _invoiceRepository.GetAll()
                            .Where(i => i.Products.Any(p => p.Id == x.Id))
                            .Select(i => i.CreationDate)
                            .FirstOrDefault();

                        var lastMovement = _productMovementRepository.GetAll()
                            .Where(m => m.Product.Id == x.Id)

                            .FirstOrDefault();
                        var movementCount = _productMovementRepository.GetAll()
        .Count(m => m.Product.Id == x.Id);
                        var debitingWarehouseId = lastMovement.DestinationWarehouseId;
                        if (movementCount > 1)
                        {
                            debitingWarehouseId = lastMovement.DestinationWarehouseId;
                        }
                        else
                        {
                            debitingWarehouseId = lastMovement.SourceWarehouseId;
                        }
                        //Тут тоже исправить нужно
                        var debitingWarehouseName = _warehouseRepository.GetAll()
          .Where(w => w.Id == debitingWarehouseId)
          .Select(w => w.Name)
          .FirstOrDefault();

                        return new ProductDebitingViewModel()
                        {
                            Id = x.Id,
                            ProductName = x.NameProduct,
                            Inventory = x.InventoryCode,
                            CommentsDebiting = x.Comments,
                            DataEntrance = invoice.ToString("g"),
                            DateDebiting = x.TimeDebbiting.Value.ToString("g"),
                            OriginalWarehouse = x.Warehouse.Name,
                            DebitingWarehouse = debitingWarehouseName
                        };
                            }).ToList(),
                    }).ToList();




                    return new BaseResponse<IEnumerable<ProductDebitingWarehouseViewModel>>()
                    {
                        Data = result,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<IEnumerable<ProductDebitingWarehouseViewModel>>()
                {
                    StatusCode = StatusCode.NotFind,
                    Description = $"Нет списанного товара"
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<ProductDebitingWarehouseViewModel>>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"{ex.Message}"
                };

            }



        }
    }
}



