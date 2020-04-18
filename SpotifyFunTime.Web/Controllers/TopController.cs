using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TopController : BaseRequestController
    {
        private const int MAX_TOP_COUNT = 5;
        private readonly ISpotifyService _service;

        public TopController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        [HttpGet]
        [Route("tracks/{timeRange}")]
        public async Task<IActionResult> GetUserTopTracks(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopTracks(Tokens, timeRange, MAX_TOP_COUNT));

        [HttpGet]
        [Route("artists/{timeRange}")]
        public async Task<IActionResult> GetUserTopArtists(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopArtists(Tokens, timeRange, MAX_TOP_COUNT));

        [HttpGet]
        [Route("genres/{timeRange}")]
        public async Task<IActionResult> GetUserTopGenres(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopGenres(Tokens, timeRange, MAX_TOP_COUNT));
    }
}