﻿using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Warehouse;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class WarehouseService : IWarehouseService
    {
        private IBaseRepository<Warehouse> _warehouseRepository;
        private IBaseRepository<Products> _products;

        public WarehouseService(IBaseRepository<Warehouse> warehouse, IBaseRepository<Products> products)
        {
            _warehouseRepository = warehouse;
            _products = products;
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
                    var Products = await _products.GetAll()
                        .Where(wh => wh.Warehouse == Warehouse) //Фильтрация по складу
                        .GroupBy(x => x.NameProduct) //Группировка по 
                        .Select(group => new ProductinWarehouseViewModel()
                        {
                            NameProduct = group.Key,
                            CodeProduct = group.First().InventoryCode,
                            TotalCountWarehouse = group.Count(),
                            AvailableCount = group.Count(x => x.UserId == null) //Подсчитываем доступное количество товаров
                        })
                        .ToListAsync();
                    return new DataTableResponse()
                    {
                        Data = Products,
                        
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
    }
}
