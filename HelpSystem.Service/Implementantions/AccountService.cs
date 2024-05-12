using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Helpers;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Account;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpSystem.Service.Implementantions
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<User> _useRepository;
        private readonly IBaseRepository<Profile> _profileRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        public AccountService(IBaseRepository<User> useRepository, IBaseRepository<Profile> profileRepository, IBaseRepository<Role> roleRepository)
        {
            _useRepository = useRepository;
            _profileRepository = profileRepository;
            _roleRepository = roleRepository;
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterVIewModel model)
        {
            try
            {
                var User = _useRepository.GetAll().FirstOrDefault(x => x.Login == model.Login);
                if (User != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь с текущим логином уже существует",
                        StatusCode = StatusCode.UserIsRegistered
                    };
                }
                else
                {



                    var DefaultRole = UserRoleType.User;
                    var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.RoleType == DefaultRole);
                    if (role != null)
                    {


                        User = new User()
                        {
                            Login = model.Login,
                            Name = model.Name,
                            Password = HashPassword.HashPassowrds(model.Password),
                            Roles = role

                        };

                    }

                    await _useRepository.Create(User);

                    var profile = new Profile()
                    {
                        UserId = User.Id,
                        Name = User.Name,
                        Email = model.Email
                    };
                    await _profileRepository.Create(profile);



                    var Result = Authenticate(User);
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Data = Result,
                        Description = "Пользователь успешно зарегистрирован",
                        StatusCode = StatusCode.Ok
                    };
                }
            }
            catch (Exception Ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = Ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                var user = await _useRepository.GetAll().FirstOrDefaultAsync(x => x.Login == model.Login);
                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFind

                    };
                }
                if (user.Password != HashPassword.HashPassowrds(model.Password))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный логин или пароль",
                        StatusCode = StatusCode.NotFind
                    };

                }
                var result = Authenticate(user);
                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }


        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Изменили на ClaimTypes.NameIdentifier
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);
        }
    }
}
