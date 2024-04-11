using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Product.ProductAllInfo;

namespace HelpSystem.Service.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Метод,  который будет возвращать в частичное преставление конкретные товары, привязанные к накладной
        /// </summary>
        /// <returns></returns>
        //Task<BaseResponse<IEnumerable<Products>>> GetPartialProduct(Guid id);

        Task<BaseResponse<Dictionary<int, string>>> GetProductNotUser(string term);
        /// <summary>
        /// Метод получает список товаров, включая привязанных 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        Task<BaseResponse<Dictionary<int, string>>> GetAllProductsWith(string term);
        Task<BaseResponse<IEnumerable<Products>>> CreateProduct(List<ProductViewModel> positions);

        /// <summary>
        /// Метод создания привязки товара к пользователю
        /// В качестве параметров передётся ID заявки, от туда вытягивается юзверь, товар и комментарий, если нужно.
        /// </summary>
        /// <param name="StatId"></param>
        /// <param name="ProductId"></param>
        /// <param name="Comments"></param>
        /// <returns>Привязанный товар</returns>
        Task<BaseResponse<Products>> BindingProduct(Guid StatId,int ProductId,string?  Comments);
        /// <summary>
        /// Метод отвязки товара от пользователя
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<BaseResponse<IEnumerable<Products>>> UnBindingProduct( List<UnbindingProductViewModel>   product, Guid ProfileId);


        /// <summary>
        /// Метод получения всей доступной информации о товаре
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IBaseResponse<MainProductInfo>> GetMainProductInfo(int id);
      
    }
}
