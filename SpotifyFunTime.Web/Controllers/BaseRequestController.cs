using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SpotifyFunTime.Application;
using SpotifyFunTime.Application.Utilities;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    public class BaseRequestController : BaseController
    {
        private readonly IContentCache _cache;
        private readonly ISpotifyAuthClient _authClient;
        
        public BaseRequestController(IContentCache cache, ISpotifyAuthClient authClient)
        {
            _cache = cache;
            _authClient = authClient;
        }

        protected async Task<IActionResult> MakeCachedRequest<T>(Func<Task<ApiResponse<T>>> func)
        {
            var cacheKey = GetCacheKey(func.Method);

            if (!_cache.TryGet(cacheKey, out ApiResponse<T> cacheEntry))
            {
                return await MakeRequest(func, shouldCache: true);
            }

            return Ok(cacheEntry);
        }

        protected async Task<IActionResult> MakeRequest<T>(Func<Task<ApiResponse<T>>> func, bool shouldCache = false, bool isRetry = false)
        {
            if (string.IsNullOrWhiteSpace(Tokens.AccessToken))
            {
                return Unauthorized();
            }
            
            var result = await func();

            switch (result.StatusCode)
            {
                case HttpStatusCode.Unauthorized when isRetry:
                    return Unauthorized();
                case HttpStatusCode.Unauthorized:
                {
                    var tokenSet = await _authClient.RefreshToken(Tokens.RefreshToken);
                    SetSessionInfo(tokenSet);
                    await MakeRequest<T>(func, true);
                    break;
                }
                case HttpStatusCode.OK:
                    if (shouldCache) {
                        var cacheKey = GetCacheKey(func.Method);
                        _cache.Set(cacheKey, result);
                    }

                    return Ok(result);
            }
            
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        private string GetCacheKey(MethodInfo info)
        {
            var controllerParts = info.DeclaringType.FullName.Split('.');
            var controllerName = controllerParts[controllerParts.Length - 1];
            var fullMethodIdentifier = $"{controllerName}_{info.Name}";
            var cacheKey = $"{fullMethodIdentifier}_{Tokens.AccessToken}";

            return cacheKey;
        }
    }
}