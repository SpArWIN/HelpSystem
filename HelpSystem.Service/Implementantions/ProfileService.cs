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
                    .Where(x => x.User.Id == Guid)
                    .GroupBy(x=> new {x.InventoryCode,  x.NameProduct} )
                    .Select(g=> new
                    {
                        NameProduct = g.Key,
                        Code = g.Key.InventoryCode,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var Profile = await _profileRepository.GetAll()
                    
                    .Select(x => new ProfileViewModel()
                    {
                        Id = x.UserId,
                        Description = x.Description,
                        Age = x.Age,
                        Surname = x.Surname,
                        LastName = x.LastName,
                        Name = x.Name,
                        UserPdocut = Product.Select(p => new BindingProductViewModel()
                        {
                            
                            NameProduct = p.NameProduct.NameProduct,
                            InventoryCod = p.NameProduct.InventoryCode,
                            TotalCount= p.Count
                        }).ToList()

                    })
                    .FirstOrDefaultAsync(x => x.Id == Guid);
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
    }
}
