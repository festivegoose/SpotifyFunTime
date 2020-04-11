using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SpotifyFunTime.Application;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpotifyController : BaseRequestController
    {
        private readonly ISpotifyService _service;

        public SpotifyController(IContentCache cache, ISpotifyAuthClient authClient, ISpotifyService service) : base(cache, authClient)
        {
            _service = service;
        }

        [HttpGet]
        [Route("welcome")]
        public async Task<IActionResult> Welcome() =>
            await MakeCachedRequest(() => _service.GetCurrentUser(Tokens));

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
    }
}