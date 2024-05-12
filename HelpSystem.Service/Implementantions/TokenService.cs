using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Token;
using HelpSystem.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Service.Implementantions
{
    public class TokenService : ITokenCacheService
    {
        private readonly IMemoryCache _cache;
        public TokenService(IMemoryCache cache)
        {
            _cache = cache;
        }
        /// <summary>
        /// Метод получения токена
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<BaseResponse<TokenInfo>> GetTokenAsync(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out TokenInfo tokenInfo))
                {
                    return Task.FromResult(new BaseResponse<TokenInfo>()
                    {
                        Data = tokenInfo,
                        StatusCode = Domain.Enum.StatusCode.Ok
                    });
                }
                return Task.FromResult(new BaseResponse<TokenInfo>()
                {
                    StatusCode = Domain.Enum.StatusCode.NotFind
                });
            }
            catch (Exception)
            {
                return Task.FromResult(new BaseResponse<TokenInfo>()
                {
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                });
            }
        }

        public Task<BaseResponse<object>> RemoveTokenAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                return Task.FromResult(new BaseResponse<object>()
                {
                    StatusCode = Domain.Enum.StatusCode.Ok
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new BaseResponse<object>()
                {
                    StatusCode = Domain.Enum.StatusCode.InternalServerError,
                    Description = ex.Message
                });
            }
        }

        public Task<BaseResponse<object>> SetTokenAsync(string key, string value, TimeSpan expirationTime)
        {
            try
            {
                TokenInfo tokenInfo = new TokenInfo
                {
                    Token = key,
                    ExpirationTime = expirationTime
                };
                _cache.Set(key, tokenInfo, expirationTime);
                return Task.FromResult(new BaseResponse<object>()
                {
                    StatusCode = Domain.Enum.StatusCode.Ok,
                    Data = tokenInfo,
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new BaseResponse<object>()
                {
                    StatusCode = Domain.Enum.StatusCode.InternalServerError,
                    Description = ex.Message,
                });

            }
        }
    }
}
