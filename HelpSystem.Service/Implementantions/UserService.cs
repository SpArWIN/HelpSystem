using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Extension;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Users;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _accountUseRepository;

        public UserService(IBaseRepository<User> repository)
        {
            _accountUseRepository = repository;
        }
        public async Task<BaseResponse<IEnumerable<UsersViewModel>>> GetAllUsers()
        {
            try
            {
                var users = await _accountUseRepository.GetAll().Include(u => u.Roles) // Загрузка связанных данных Roles
                    .Include(u => u.Profile).ToListAsync();

                var UserViews = users.Select(x => new UsersViewModel
                {
                    Login = x.Login,
                    Name = x.Name,
                    Surname = x.Profile.Surname,
                    LastName = x.Profile.LastName,
                    Roles = x.Roles.RoleType.GetDisplayName(),
                    Age = x.Profile.Age,

                }).ToList();
                return new BaseResponse<IEnumerable<UsersViewModel>>()
                {
                    Data = UserViews,
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<UsersViewModel>>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = ex.Message
                };
            }
        }
    }
}
