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
                    // Получаем список ID всех товаров, которые были перемещены с этого склада
                    var movementProducts = await _productMovementRepository.GetAll()
                        .Where(m => m.DestinationWarehouseId == Warehouse.Id)
                        .Select(m => m.ProductId)
                        .ToListAsync();
                    // Фильтруем товары на выбранном складе: оставляем только те товары, которые не были перемещены

                    var AvailableProducts = productsOnWarehouse.Where(p => !movementProducts.Contains(p.Id));


                    // Группируем товары по их наименованию
                    var groupedProducts = AvailableProducts
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
                        Data = groupedProducts,
                        
                    };

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
        public  async Task<BaseResponse<IEnumerable<TransferProductViewModel>>> GetProductsDetails(Guid WhId)
        {
            try
            {
                var Warehouse = await _warehouseRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == WhId);
                if (Warehouse != null)
                {
                    //Получаем все товары на складе
                    var productsOnWarehouse = await _products.GetAll()
                        .Where(p => p.Warehouse == Warehouse)
                        .ToListAsync();
                    //Получаем список товаров, которые вообще были перемещены с этого склада
                    var movementProducts = await _productMovementRepository.GetAll()
                        .Where(m => m.DestinationWarehouseId == Warehouse.Id) // Фильтруем по ID склада товара
                        .Select(m => m.ProductId)
                        .ToListAsync();

                    // Фильтруем товары на выбранном складе: оставляем только те товары, которые не были перемещены и не закреплены за пользователем
                    var availableProducts = productsOnWarehouse.Where(p => !movementProducts.Contains(p.Id) && p.UserId == null);

                    //Сразу же передадим список складов, кроме текущего.
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
                        Data  = productDetails,
                        StatusCode = StatusCode.Ok
                    };
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
