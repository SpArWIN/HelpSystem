using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;


namespace HelpSystem.Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<IBaseResponse<Invoice>> CreateInvoice(string NumberDocument, List<ProductViewModel> positions);
        //Добавить потом остальные методы
    }
}
