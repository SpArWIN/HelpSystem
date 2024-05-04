using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Provider;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class ProviderService : IProviderService
    {
        private readonly IBaseRepository<Provider> _Providerrepository;

        public ProviderService(IBaseRepository<Provider> provider)
        {
            _Providerrepository = provider;
        }

        public async Task<BaseResponse<Provider>> CreateProvider(ProviderViewModel model)
        {
            try
            {

                var Providers = await _Providerrepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == model.ProviderName);
                if (Providers == null)
                {
                    var NewProvider = new Provider()
                    {
                        Name = model.ProviderName,
                        IsFreeZing = false
                    };
                    await _Providerrepository.Create(NewProvider);
                    return new BaseResponse<Provider>()
                    {
                        Data = NewProvider,
                        Description = "Поставщик успешно добавлен",
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<Provider>()
                {
                    Description = "Поставщик уже существует",
                    StatusCode = StatusCode.UnCreated
                };
            }
            catch (Exception e)
            {
                return new BaseResponse<Provider>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<Provider>> DeleteProvider(Guid id)
        {
            try
            {
                var DelProvider = await _Providerrepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (DelProvider == null)
                {
                    return new BaseResponse<Provider>()
                    {
                        Description = "Объект не найден",
                        StatusCode = StatusCode.NotFind
                    };

                }

                await _Providerrepository.Delete(DelProvider);
                return new BaseResponse<Provider>()
                {
                    Description = $"Поставщик {DelProvider.Name} удалён",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception e)
            {
                return new BaseResponse<Provider>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<Provider>> FreezeProvider(Guid id)
        {
            try
            {
                var Provider = await _Providerrepository.GetAll()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
                if (Provider == null)
                {
                    return new BaseResponse<Provider>()
                    {
                        Description = "Поставщик не найден",
                        StatusCode = StatusCode.NotFind
                    };
                }

                Provider.IsFreeZing = true;
                await _Providerrepository.Update(Provider);
                return new BaseResponse<Provider>()
                {
                    Description = $"Поставщик {Provider.Name} успешно заморожен.",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {

                return new BaseResponse<Provider>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }

        }
        public async Task<BaseResponse<Provider>> UnFreezeProvider(Guid id)
        {
            try
            {
                var UnFreezeProvider = await _Providerrepository.GetAll()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
                if (UnFreezeProvider == null)
                {
                    return new BaseResponse<Provider>()
                    {
                        Description = "Поставщик не найден",
                        StatusCode = StatusCode.NotFind
                    };
                }
                UnFreezeProvider.IsFreeZing = false;

                await _Providerrepository.Update(UnFreezeProvider);

                return new BaseResponse<Provider>()
                {
                    Description = $"Поставщик {UnFreezeProvider.Name} успешно разморожен.",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Provider>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<ProviderViewModel>>> GetAllProvider()
        {
            try
            {
                var AllProviders = await _Providerrepository.GetAll()
                    .Select(x => new ProviderViewModel()
                    {
                        ProviderName = x.Name,
                        ProviderId = x.Id,
                        IsFreeze = x.IsFreeZing
                    })
                    .ToListAsync();

                if (AllProviders.Count == 0)
                {
                    return new BaseResponse<IEnumerable<ProviderViewModel>>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Нет поставщиков"
                    };
                }

                return new BaseResponse<IEnumerable<ProviderViewModel>>()
                {
                    Data = AllProviders,
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<ProviderViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ProviderViewModel>> GetProviderCurrent(Guid id)
        {
            try
            {
                var Response = await _Providerrepository.GetAll()
                    .Select(x => new ProviderViewModel
                    {
                        ProviderId = x.Id,
                        ProviderName = x.Name,
                    })
                    .FirstOrDefaultAsync(x => x.ProviderId == id);
                if (Response != null)
                {
                    return new BaseResponse<ProviderViewModel>()
                    {
                        Data = Response,
                        StatusCode = StatusCode.Ok,

                    };
                }

                return new BaseResponse<ProviderViewModel>()
                {
                    StatusCode = StatusCode.NotFind,
                    Description = "Поставщик не найден",
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse<ProviderViewModel>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<Provider>> SaveProvider(ProviderViewModel model)
        {
            try
            {


                var UpdateProvider = await _Providerrepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == model.ProviderId);

                if (UpdateProvider.Name != model.ProviderName)
                {
                    // Проверяем, существует ли поставщик с таким же наименованием, которое хотим поменять
                    var ExistProvider = await _Providerrepository
                        .GetAll().FirstOrDefaultAsync(x => x.Name == model.ProviderName);
                    if (ExistProvider != null)
                    {
                        return new BaseResponse<Provider>()
                        {
                            Description = "Поставщик с таким наименованием уже существует",
                            StatusCode = StatusCode.UnChanched
                        };
                    }



                    UpdateProvider.Name = model.ProviderName;
                    await _Providerrepository.Update(UpdateProvider);
                    return new BaseResponse<Provider>()
                    {
                        Description = "Наименование поставщика успешно изменено",
                        Data = UpdateProvider,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<Provider>()
                {
                    StatusCode = StatusCode.UnChanched,
                    Description = "Не наблюдаю никаких изменений"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Provider>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };

            }
        }


    }
}
