using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Warehouse;

namespace HelpSystem.Service.Interfaces
{
    public interface IWarehouseService
    {
        /// <summary>
        /// Создание склада
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BaseResponse<Warehouse>> CreateWarehouse(WarehouseViewModel model);
        /// <summary>
        /// Удаление склада
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<Warehouse>> DeleteWarehouse(Guid id);
        /// <summary>
        /// Все склады
        /// </summary>
        /// <returns>Возвращает список всех складов</returns>
        Task<BaseResponse<IEnumerable<WarehouseViewModel>>> GetAllWarehouse();

    }
}
