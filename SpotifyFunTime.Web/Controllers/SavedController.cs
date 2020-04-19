using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/saved")]
    [ApiController]
    public class SavedController : BaseRequestController
    {
        private const int MAX_TOP_COUNT = 5;
        private readonly ISpotifyService _service;

        public SavedController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        /// <summary>
        /// Returns top 5 most popular tracks saved to current user's library.
        /// </summary>
        [HttpGet]
        [Route("popular-tracks")]
        [ProducesResponseType(typeof(ApiResponse<List<SavedTrack>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMostPopularTracks() =>
            await MakeRequest(() => _service.GetUserMostPopularSavedTracks(Tokens, MAX_TOP_COUNT));

        /// <summary>
        /// Returns top 5 least popular tracks saved to current user's library.
        /// </summary>
        [HttpGet]
        [Route("unpopular-tracks")]
        [ProducesResponseType(typeof(ApiResponse<List<SavedTrack>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLeastPopularTracks() =>
            await MakeRequest(() => _service.GetUserLeastPopularSavedTracks(Tokens, MAX_TOP_COUNT));

        /// <summary>
        /// Returns top 5 most popular artists with tracks saved to current user's library.
        /// </summary>
        [HttpGet]
        [Route("popular-artists")]
        [ProducesResponseType(typeof(ApiResponse<List<Artist>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMostPopularSavedArtists() =>
            await MakeRequest(() => _service.GetUserMostPopularSavedArtists(Tokens, MAX_TOP_COUNT));

        /// <summary>
        /// Returns top 5 least popular artists with tracks saved to current user's library.
        /// </summary>
        [HttpGet]
        [Route("unpopular-artists")]
        [ProducesResponseType(typeof(ApiResponse<List<Artist>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLeastPopularSavedArtists() =>
            await MakeRequest(() => _service.GetUserLeastPopularSavedArtists(Tokens, MAX_TOP_COUNT));

        /// <summary>
        /// Returns a dictionary of the current + last 11 months, and how many songs were saved
        /// to the current user's library during each.
        /// </summary>
        [HttpGet]
        [Route("monthly-tracks")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSavedTracksByMonth() =>
            await MakeRequest(() => _service.GetUserSavedTracksByMonth(Tokens));

        /// <summary>
        /// Returns all tracks in current user's library.
        /// </summary>
        [HttpGet]
        [Route("all-tracks")]
        [ProducesResponseType(typeof(ApiResponse<List<SavedTrack>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSavedTracks() =>
            await MakeRequest(() => _service.GetUserSavedTracks(Tokens));

        //TODO: Add artist that you've been listening to longest (occurs more than once, with longest time span between oldest
        //      and newest occurrences)
    }
}