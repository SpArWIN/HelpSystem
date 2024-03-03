using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Provider;

namespace HelpSystem.Service.Interfaces
{
    public interface IProviderService
    {
        Task<BaseResponse<Provider>> CreateProvider(ProviderViewModel model);
        Task<BaseResponse<Provider>>
    }
}
