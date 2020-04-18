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

        private readonly ISpotifyService _service;

        public SpotifyController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        [HttpGet]
        [Route("welcome")]
        public async Task<IActionResult> Welcome() =>
            await MakeRequest(() => _service.GetCurrentUser(Tokens));

        /***** FROM TOP PLAYED *****/

        [HttpGet]
        [Route("top-tracks/{timeRange}")]
        public async Task<IActionResult> GetUserTopTracks(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopTracks(Tokens, timeRange, MAX_TOP_COUNT));

        [HttpGet]
        [Route("top-artists/{timeRange}")]
        public async Task<IActionResult> GetUserTopArtists(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopArtists(Tokens, timeRange, MAX_TOP_COUNT));

        [HttpGet]
        [Route("top-genres/{timeRange}")]
        public async Task<IActionResult> GetUserTopGenres(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopGenres(Tokens, timeRange, MAX_TOP_COUNT));

        /***** FROM SAVED TRACKS / LIBRARY *****/

        [HttpGet]
        [Route("popular-tracks")]
        public async Task<IActionResult> GetUserMostPopularTracks() =>
            await MakeRequest(() => _service.GetUserMostPopularTracks(Tokens, MAX_TOP_COUNT));

        [HttpGet]
        [Route("unpopular-tracks")]
        public async Task<IActionResult> GetUserLeastPopularTracks() =>
            await MakeRequest(() => _service.GetUserLeastPopularTracks(Tokens, MAX_TOP_COUNT));

        [HttpGet]
        [Route("monthly-saved-tracks")]
        public async Task<IActionResult> GetUserMonthlySavedTracks() =>
            await MakeRequest(() => _service.GetUserSavedTracksByMonth(Tokens));

            

        // [HttpGet]
        // [Route("saved-tracks")]
        // public async Task<IActionResult> GetUserSavedTracks() =>
        //     await MakeRequest(() => _service.GetUserSavedTracks(Tokens));

        // NOT ACCURATE
        // [HttpGet]
        // [Route("last-ten")]
        // public async Task<IActionResult> GetLastTenPlayedTracks() =>
        //     await MakeRequest(() => _service.GetLastTenPlayedTracks(Tokens));




        //TODO: Most popular artists in saved tracks

        //TODO: Favorite genres in saved tracks

        //TODO: Chart with songs saved by month (last year)

        //TODO: Audio features of a song

        //TODO: Audio features min/max in saved tracks
    }
}