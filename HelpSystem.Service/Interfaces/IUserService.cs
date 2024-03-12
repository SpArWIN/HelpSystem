using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Users;

namespace HelpSystem.Service.Interfaces
{
    public interface IUserService
    {
        //реализацию брать из AccountService, 
        public Task<BaseResponse<IEnumerable<UsersViewModel>>> GetAllUsers();
      
    }
}
