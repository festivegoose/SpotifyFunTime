using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/top")]
    [ApiController]
    public class TopController : BaseRequestController
    {
        private const int MAX_TOP_COUNT = 5;
        private readonly ISpotifyService _service;

        public TopController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        /// <summary>
        /// Returns top 5 most played tracks within a given time range.
        /// </summary>
        /// <param name="timeRange">Allowed values: short_term | medium_term | long_term</param>
        [HttpGet]
        [Route("tracks/{timeRange}")]
        [ProducesResponseType(typeof(ApiResponse<List<Track>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserTopTracks(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopTracks(Tokens, timeRange, MAX_TOP_COUNT));

        /// <summary>
        /// Returns top 5 most played artists within a given time range.
        /// </summary>
        /// <param name="timeRange">Allowed values: short_term | medium_term | long_term</param>
        [HttpGet]
        [Route("artists/{timeRange}")]
        [ProducesResponseType(typeof(ApiResponse<List<Artist>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserTopArtists(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopArtists(Tokens, timeRange, MAX_TOP_COUNT));

        /// <summary>
        /// Returns dictionary of top 5 most played genres played within a
        /// given time range, and number of occurrences among top 50 most played
        /// artists.
        /// </summary>
        /// <param name="timeRange">Allowed values: short_term | medium_term | long_term</param>
        [HttpGet]
        [Route("genres/{timeRange}")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserTopGenres(string timeRange) =>
            await MakeRequest(() => _service.GetUserTopGenres(Tokens, timeRange, MAX_TOP_COUNT));
    }
}