using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Profile;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class ProfileService : IProfileService
    {
        private readonly IBaseRepository<Profile> _profileRepository;
        //Добавлю репозиторий товаров, чтобы вынуть связанные товары
        private readonly IBaseRepository<Products> _productsRepository;
        public ProfileService(IBaseRepository<Profile> profileRepository, IBaseRepository<Products> productsRepository)
        {
            _profileRepository = profileRepository;
            _productsRepository = productsRepository;
        }

        public async Task<BaseResponse<ProfileViewModel>> GetProfile(Guid Guid)
        {
            try
            {
                //В профиль также закинем список товаров связанных с этим пользователем
                //Цепляем id, чтобы его потом можно было открепить от юзверя


                var Product = await _productsRepository.GetAll()
                    .Where(x => x.UserId == Guid)
                    .Select(g => new BindingProductViewModel()
                    {
                        Id = g.Id,
                        InventoryCod = g.InventoryCode,
                        NameProduct = g.NameProduct,
                        TotalCount = 1
                    }).ToListAsync();
                int sumTotalProducts = Product.Sum(p => p.TotalCount);

                var Profile = await _profileRepository.GetAll()
                    .Select(x => new ProfileViewModel()
                    {
                        Id = x.UserId,
                        Description = x.Description,
                        Age = x.Age,
                        Surname = x.Surname,
                        LastName = x.LastName,
                        Name = x.Name,
                        UserPdocut = Product,
                        SumTotalProducts = sumTotalProducts
                    }).FirstOrDefaultAsync(x=>x.Id == Guid);



                return new BaseResponse<ProfileViewModel>()
                {
                    Data = Profile,
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<ProfileViewModel>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = ex.Message
                };
            }

        }

        public async Task<BaseResponse<Profile>> Save(ProfileViewModel model)
        {
            try
            {
                var profile = _profileRepository.GetAll()
                    .FirstOrDefault(x => x.UserId == model.Id);



                if (profile.Age != model.Age ||
                    profile.Surname != model.Surname ||
                    profile.LastName != model.LastName ||
                    profile.Description != model.Description || profile.Name != model.Name)
                {
                    profile.Age = model.Age;
                    profile.Surname = model.Surname;
                    profile.LastName = model.LastName;
                    profile.Description = model.Description;
                    profile.Name = model.Name;
                    await _profileRepository.Update(profile);
                    return new BaseResponse<Profile>()
                    {
                        Data = profile,
                        Description = "Данные были успешно изменены",
                        StatusCode = StatusCode.Ok
                    };

                }
                else
                {
                    return new BaseResponse<Profile>()
                    {
                        StatusCode = StatusCode.UnChanched,
                        Description = "Не вижу никаких изменений"
                    };
                }


            }
            catch (Exception ex)
            {
                return new BaseResponse<Profile>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Ошибка изменения данных: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<Dictionary<Guid, string>>> GetUser(string term)
        {
            try
            {
                //Мы находим только пользователей, которые имеют роль "Пользователя"

                var Response = await _profileRepository.GetAll()
                    .Include(u => u.User)
                    .Where(u=>u.User.RoleId ==1)
                    .Where(x => EF.Functions.Like(x.Name, $"%{term}%") || EF.Functions.Like(x.LastName, $"%{term}%") ||
                                EF.Functions.Like(x.Surname, $"%{term}%") ||
                                EF.Functions.Like(x.User.Login, $"%{term}%"))
                    .ToListAsync();

                if (Response.Any())
                {
                    var usersDictionary = Response.ToDictionary(
                        x => x.UserId, 
                        x => $"{x.LastName} {x.Name} {x.Surname} ({x.User.Login})" // Value: Full Name + Login
                    );
                    return new BaseResponse<Dictionary<Guid, string>>()
                    {
                        Data = usersDictionary,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<Dictionary<Guid, string>>()
                {
                    StatusCode = StatusCode.NotFind,
                  
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Dictionary<Guid, string>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
