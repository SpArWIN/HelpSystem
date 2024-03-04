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
                        Name = model.ProviderName
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

        public async Task<BaseResponse<IEnumerable<ProviderViewModel>>> GetAllProvider()
        {
            try
            {
                var AllProviders = await _Providerrepository.GetAll()
                    .Select(x => new ProviderViewModel()
                    {
                        ProviderName = x.Name,
                        ProviderId = x.Id
                    })
                    .ToListAsync();

                if (AllProviders.Count == 0)
                {
                    return new BaseResponse<IEnumerable<ProviderViewModel>>()
                    {
                        Description = "Спиоск поставщиков пуст",
                        StatusCode = StatusCode.NotFind
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
    }
}
