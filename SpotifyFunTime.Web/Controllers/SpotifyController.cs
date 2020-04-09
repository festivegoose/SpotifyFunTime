using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpotifyController : BaseController
    {
        private readonly ISpotifyAuthClient _authClient;
        private readonly ISpotifyService _service;

        public SpotifyController(ISpotifyAuthClient authClient, ISpotifyService service)
        {
            _authClient = authClient;
            _service = service;
        }

        [HttpGet]
        [Route("welcome")]
        public async Task<IActionResult> Welcome() =>
            await MakeRequest(() => _service.GetCurrentUser(Tokens));

        // [HttpGet]
        // [Route("last-ten")]
        // public async Task<IActionResult> GetLastTenPlayedTracks() =>
        //     await MakeRequest(() => _service.GetLastTenPlayedTracks(Tokens));

        [HttpGet]
        [Route("top-tracks/{timeRange}")]
        public async Task<IActionResult> GetTopTracks(string timeRange, [FromQuery]int limit = 5) =>
            await MakeRequest(() => _service.GetTopTracks(Tokens, timeRange, limit));

        [HttpGet]
        [Route("top-artists/{timeRange}")]
        public async Task<IActionResult> GetTopArtists(string timeRange, [FromQuery]int limit = 5) =>
            await MakeRequest(() => _service.GetTopArtists(Tokens, timeRange, limit));

        private async Task<IActionResult> MakeRequest<T>(Func<Task<ApiResponse<T>>> func, bool isRetry = false)
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
                    return Ok(result);
            }
            
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}