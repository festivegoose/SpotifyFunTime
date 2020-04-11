using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    public class BaseRequestController : BaseController
    {
        private readonly ISpotifyAuthClient _authClient;
        
        public BaseRequestController(ISpotifyAuthClient authClient)
        {
            _authClient = authClient;
        }

        protected async Task<IActionResult> MakeRequest<T>(Func<Task<ApiResponse<T>>> func, bool isRetry = false)
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
                    await MakeRequest<T>(func, isRetry: true);
                    break;
                }
                case HttpStatusCode.OK:
                    return Ok(result);
            }
            
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}