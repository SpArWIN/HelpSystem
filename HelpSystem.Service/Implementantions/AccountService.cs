﻿using HelpSystem.DAL.Interfasces;
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
        private readonly ITokenCacheService _tokenCacheService;
        public AccountService(IBaseRepository<User> useRepository, IBaseRepository<Profile> profileRepository, IBaseRepository<Role> roleRepository, ITokenCacheService tokenCacheService)
        {
            _useRepository = useRepository;
            _profileRepository = profileRepository;
            _roleRepository = roleRepository;
            _tokenCacheService = tokenCacheService;
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


                    var GetAllusers = await _useRepository.GetAll()
                        .Include(x => x.Profile)
                        .ToListAsync();
                    bool isEmail = GetAllusers.Any(x => x.Profile.Email == model.Email);
                    if (isEmail)
                    {
                        return new BaseResponse<ClaimsIdentity>()
                        {
                            Description = "Адрес электронной почты уже используется. Пожалуйста, введите другой адрес.",
                            StatusCode = StatusCode.UnCreated
                        };
                    }

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
                        await _useRepository.Create(User);
                    }



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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);
        }

        public async Task<BaseResponse<User>> RecoveryPassword(RecoveryProfile model, string Token)
        {
            try
            {
                var User = await _useRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == model.UserId);

                var Tok = await _tokenCacheService.GetTokenAsync(Token);

                if (Tok.StatusCode == StatusCode.Ok)
                {
                    TimeSpan CreateToken = Tok.Data.ExpirationTime;
                    TimeSpan tokenAge = DateTime.Now - DateTime.Now.Add(Tok.Data.ExpirationTime);
                    if (tokenAge > CreateToken)
                    {
                        //Время жизни токена истекло
                        await _tokenCacheService.RemoveTokenAsync(Token);
                    }
                }

                //Проверим действителен ли токен

                if (User == null)
                {
                    return new BaseResponse<User>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFind
                    };

                }
                User.Password = HashPassword.HashPassowrds(model.NewPassword);

                await _useRepository.Update(User);

                return new BaseResponse<User>()
                {
                    Description = "Пароль был успешно изменён!",
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception ex)
            {

                return new BaseResponse<User>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = ex.Message
                };
            }
        }

        public async Task<BaseResponse<User>> ChangePassword(RecoveryProfile profile)
        {
            try
            {
                var User = await _useRepository.GetAll()
                    .Where(x => x.Id == profile.UserId)
                    .FirstOrDefaultAsync();
                if (User == null)
                {
                    return new BaseResponse<User>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFind
                    };
                }
                User.Password = HashPassword.HashPassowrds(profile.NewPassword);
                await _useRepository.Update(User);
                return new BaseResponse<User>()
                {

                    Description = $"Пароль успешно изменён.",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<User>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };

            }
        }




    }
}
