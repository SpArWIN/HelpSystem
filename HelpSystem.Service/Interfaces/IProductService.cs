using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;

namespace HelpSystem.Service.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Метод,  который будет возвращать в частичное преставление конкретные товары, привязанные к накладной
        /// </summary>
        /// <returns></returns>
        //Task<BaseResponse<IEnumerable<Products>>> GetPartialProduct(Guid id);

        Task<BaseResponse<Dictionary<Guid, string>>> GetProduct(string term);

      
        Task<BaseResponse<IEnumerable<Products>>> CreateProduct(List<ProductViewModel> positions);
    }
}
