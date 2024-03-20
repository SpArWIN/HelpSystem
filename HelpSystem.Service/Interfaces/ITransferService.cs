using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Transfer;

namespace HelpSystem.Service.Interfaces
{
    public interface ITransferService
    {
        /// <summary>
        /// Метод перемещения товара с одного склада на другой
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BaseResponse<ProductMovement>> AddTransfer(TransferViewModel model);

        Task<BaseResponse<ProductMovement>> GetCurrentPositionProduct(Guid ProductId);
    }
}
