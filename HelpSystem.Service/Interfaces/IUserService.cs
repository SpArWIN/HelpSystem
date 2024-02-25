using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Users;

namespace HelpSystem.Service.Interfaces
{
    public interface IUserService
    {
        //реализацию брать из AccountService, 
        public Task<BaseResponse<IEnumerable<UsersViewModel>>> GetAllUsers();
    }
}
