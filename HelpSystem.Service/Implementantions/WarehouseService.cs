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

        public WarehouseService(IBaseRepository<Warehouse> warehouse)
        {
            _warehouseRepository = warehouse;
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
                        WarehouseName = x.Name
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
    }
}
