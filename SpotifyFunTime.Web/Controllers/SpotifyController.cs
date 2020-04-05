using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SpotifyController : BaseController
    {
        private readonly ISpotifyAuthClient _authClient;
        private readonly ISpotifyService _service;

        public SpotifyController(ISpotifyAuthClient authClient, ISpotifyService service)
        {
            _authClient = authClient;
            _service = service;
        }

        [Route("welcome")]
        public async Task<IActionResult> Welcome()
        {
            return await MakeRequest(() => _service.GetCurrentUser(Tokens));
        }

        private async Task<IActionResult> MakeRequest<T>(Func<Task<ApiResponse<T>>> func, bool isRetry = false)
        {
            var result = await func();

            var shouldRefresh = result.StatusCode == HttpStatusCode.Unauthorized && !isRetry;
            if (shouldRefresh)
            {
                var tokenSet = await _authClient.RefreshToken(Tokens.RefreshToken);
                SetSessionInfo(tokenSet);

                result = await func();
            }

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/login");
            }

            return Ok(result);
        }
    }
}