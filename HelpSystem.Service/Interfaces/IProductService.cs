﻿using HelpSystem.Domain.Entity;
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

        /// <summary>
        /// Метод создания привязки товара к пользователю
        /// </summary>
        /// <param name="StatId"></param>
        /// <param name="ProductId"></param>
        /// <param name="Comments"></param>
        /// <returns>Привязанный товар</returns>
        Task<BaseResponse<Products>> BindingProduct(Guid StatId,Guid ProductId,string?  Comments);
        /// <summary>
        /// Метод отвязки товара от пользователя
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<BaseResponse<IEnumerable<Products>>> UnBindingProduct(UnbindingProductViewModel  product);
    }
}
