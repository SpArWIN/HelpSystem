using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Token;

namespace HelpSystem.Service.Interfaces
{
    //Так как не хочется создавать ссылки и хранить их в базе данных, нужно сделать сервис хранения токенов
    //После перехода по ссылке и успешного ответа, мы удалим его
    public interface ITokenCacheService
    {
        Task<BaseResponse<object>> SetTokenAsync(string key, string value, TimeSpan expirationTime);
        Task<BaseResponse<TokenInfo>> GetTokenAsync(string key);
        Task<BaseResponse<object>> RemoveTokenAsync(string key);
    }
}
