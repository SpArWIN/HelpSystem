using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Account;
using HelpSystem.Domain.ViewModel.Profile;

namespace HelpSystem.Service.Interfaces
{
    public interface IProfileService
    {
        Task<BaseResponse<ProfileViewModel>> GetProfile(Guid Guid);
        Task<BaseResponse<Profile>> Save(ProfileViewModel model);
        /// <summary>
        /// Метод получения пользователя в поисковике 
        /// </summary>
        /// <param name="term"></param>
        /// <returns>Возвращает список пользователей</returns>
        Task<BaseResponse<Dictionary<Guid, string>>> GetUser(string term);
     
       
    }
}
