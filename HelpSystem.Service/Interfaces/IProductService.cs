using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;

namespace HelpSystem.Service.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Метод,  возвращающий спиоск всех товаров, со всех складов 
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<IEnumerable<ProductShowViewModel>>> GetAllProduct();

        Task<BaseResponse<Products>> CreateProduct(string NumberDocument, List<ProductViewModel> positions);
    }
}
