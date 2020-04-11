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
        private const int MAX_TOP_COUNT = 5;
        private const int MAX_PAGING_LIMIT = 50;

        private readonly ISpotifyService _service;

        public SpotifyController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
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
        public async Task<IActionResult> GetUserTopTracks(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopTracks(Tokens, timeRange, MAX_TOP_COUNT));

        [HttpGet]
        [Route("top-artists/{timeRange}")]
        public async Task<IActionResult> GetUserTopArtists(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopArtists(Tokens, timeRange, MAX_TOP_COUNT));

        [HttpGet]
        [Route("saved-tracks")]
        public async Task<IActionResult> GetUserSavedTracks() =>
            await MakeRequest(() => _service.GetUserSavedTracks(Tokens, MAX_PAGING_LIMIT));
    }
}