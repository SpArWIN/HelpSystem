using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Profile;

namespace HelpSystem.Service.Interfaces
{
    public interface IProfileService
    {
        Task<BaseResponse<ProfileViewModel>> GetProfile(Guid Guid);
        Task<BaseResponse<Profile>> Save(ProfileViewModel model);
    }
}
