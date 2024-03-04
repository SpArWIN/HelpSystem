using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Buyer;

namespace HelpSystem.Service.Interfaces
{
    public interface IBuyerService
    {
        Task<BaseResponse<Buyer>> CreateBuyer(BuyerViewModel model);
        Task<BaseResponse<Buyer>> UpdateBuyer(BuyerViewModel model);
        Task<BaseResponse<Buyer>> DeleteBuyer(Guid id);
        Task <BaseResponse<IEnumerable<BuyerViewModel>>> GetAllBuyer();


    }
}
