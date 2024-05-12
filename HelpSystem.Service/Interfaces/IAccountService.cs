using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Account;
using System.Security.Claims;

namespace HelpSystem.Service.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<ClaimsIdentity>> Register(RegisterVIewModel model);
        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);
        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BaseResponse<User>> RecoveryPassword(RecoveryProfile model, string Token);

    }
}
