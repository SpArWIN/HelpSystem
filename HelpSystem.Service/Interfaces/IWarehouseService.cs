using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Transfer;
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
        /// Заморозка склада
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<Warehouse>> FreezeWarehouse(Guid id);

        /// <summary>
        /// Метод разморозки склада
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<Warehouse>> UNFreezeWarehouse(Guid id);



        /// <summary>
        /// Все склады
        /// </summary>
        /// <returns>Возвращает список всех складов</returns>
        Task<BaseResponse<IEnumerable<WarehouseViewModel>>> GetAllWarehouse();
        /// <summary>
        /// Метод получения конкретного склада
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<WarehouseViewModel>> GetWarehouse(Guid id);
        

       /// <summary>
       /// Метод получения сервисного склад, для передачи в модальное окно
       /// </summary>
       /// <returns>id</returns>
        Task<BaseResponse<Guid>>GetDetWarehouse();
        /// <summary>
        /// Метод сохранениея склада
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IBaseResponse<Warehouse>> SaveWarehouse(WarehouseViewModel model);
        /// <summary>
        /// Метод получения всех товаров по складу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResponse<IEnumerable<ProductinWarehouseViewModel>>> GetProductWarehouse(Guid id);
        /// <summary>
        /// Метод привязки товара со стороны склада
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BaseResponse<Products>> BindWarehouseProduct(BindingProductWarehouse model);

        //Метод, который получит детали товаров, т.е не по группировке, а все товары.
        Task<BaseResponse<IEnumerable<TransferProductViewModel>>> GetProductsDetails(Guid WhId);
        /// <summary>
        /// Получения всех складов, кроме текущего
        /// </summary>
        /// <param name="idGuid"></param>
        /// <returns></returns>
        Task<IBaseResponse<IEnumerable<Warehouse>>> GetNotCurrentWH(Guid idGuid);
    }
}
