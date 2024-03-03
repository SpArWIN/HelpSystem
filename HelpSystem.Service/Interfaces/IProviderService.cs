using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Provider;

namespace HelpSystem.Service.Interfaces
{
    public interface IProviderService
    {
        Task<BaseResponse<Provider>> CreateProvider(ProviderViewModel model);
        Task<BaseResponse<Provider>> DeleteProvider(Guid id);
        Task<BaseResponse<IEnumerable<ProviderViewModel>>> GetAllProvider();
    }
}
