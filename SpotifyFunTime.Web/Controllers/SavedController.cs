using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SavedController : BaseRequestController
    {
        private const int MAX_TOP_COUNT = 5;
        private readonly ISpotifyService _service;

        public SavedController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        [HttpGet]
        [Route("popular-tracks")]
        public async Task<IActionResult> GetUserMostPopularTracks() =>
            await MakeRequest(() => _service.GetUserMostPopularSavedTracks(Tokens, MAX_TOP_COUNT));

        [HttpGet]
        [Route("unpopular-tracks")]
        public async Task<IActionResult> GetUserLeastPopularTracks() =>
            await MakeRequest(() => _service.GetUserLeastPopularSavedTracks(Tokens, MAX_TOP_COUNT));

        [HttpGet]
        [Route("popular-artists")]
        public async Task<IActionResult> GetUserMostPopularSavedArtists() =>
            await MakeRequest(() => _service.GetUserMostPopularSavedArtists(Tokens, MAX_TOP_COUNT));

        [HttpGet]
        [Route("unpopular-artists")]
        public async Task<IActionResult> GetUserLeastPopularSavedArtists() =>
            await MakeRequest(() => _service.GetUserLeastPopularSavedArtists(Tokens, MAX_TOP_COUNT));

        [HttpGet]
        [Route("monthly-tracks")]
        public async Task<IActionResult> GetUserSavedTracksByMonth() =>
            await MakeRequest(() => _service.GetUserSavedTracksByMonth(Tokens));

        [HttpGet]
        [Route("all-tracks")]
        public async Task<IActionResult> GetUserSavedTracks() =>
            await MakeRequest(() => _service.GetUserSavedTracks(Tokens));
    }
}