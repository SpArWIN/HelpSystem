using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Invoice;
using HelpSystem.Domain.ViewModel.Product;


namespace HelpSystem.Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<IBaseResponse<Invoice>> CreateInvoice(string NumberDocument, List<ProductViewModel> positions);
        /// <summary>
        /// Метод отвечает за получения списка всех накаладных
        /// </summary>
        /// <returns></returns>
        Task<DataTableResponse> GetAllInvoices();
        //Добавить потом остальные методы
        /// <summary>
        /// Метод получает список товаров привязанных к накалдной
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Список товаров, связанных с накладной, отсортированной по наименованиию</returns>
        Task<IBaseResponse<IEnumerable<ProductShowViewModel>>> GetPartialProduct(Guid id);
    }
}
