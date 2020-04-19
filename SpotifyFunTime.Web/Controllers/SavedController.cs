using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Custom;
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

        /// <summary>
        /// Returns most danceable track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("most-danceable")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMostDanceableSavedTrack() =>
            await MakeRequest(() => _service.GetUserMostDanceableSavedTrack(Tokens));

        /// <summary>
        /// Returns least danceable track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("least-danceable")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLeastDanceableSavedTrack() =>
            await MakeRequest(() => _service.GetUserLeastDanceableSavedTrack(Tokens));

        /// <summary>
        /// Returns most energetic track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("most-energetic")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMostEnergeticSavedTrack() =>
            await MakeRequest(() => _service.GetUserMostEnergeticSavedTrack(Tokens));

        /// <summary>
        /// Returns least energetic track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("least-energetic")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLeastEnergeticSavedTrack() =>
            await MakeRequest(() => _service.GetUserLeastEnergeticSavedTrack(Tokens));

        /// <summary>
        /// Returns fastest tempo track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("fastest-tempo")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFastestTempoSavedTrack() =>
            await MakeRequest(() => _service.GetUserFastestTempoSavedTrack(Tokens));

        /// <summary>
        /// Returns slowest tempo track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("slowest-tempo")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSlowestTempoSavedTrack() =>
            await MakeRequest(() => _service.GetUserSlowestTempoSavedTrack(Tokens));

        /// <summary>
        /// Returns highest valence track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("highest-valence")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserHighestValenceSavedTrack() =>
            await MakeRequest(() => _service.GetUserHighestValenceSavedTrack(Tokens));

        /// <summary>
        /// Returns lowest valence track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("lowest-valence")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLowestValenceSavedTrack() =>
            await MakeRequest(() => _service.GetUserLowestValenceSavedTrack(Tokens));

        /// <summary>
        /// Returns highest loudness track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("highest-loudness")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserHighestLoudnessSavedTrack() =>
            await MakeRequest(() => _service.GetUserHighestLoudnessSavedTrack(Tokens));

        /// <summary>
        /// Returns lowest loudness track in current user's library.
        /// </summary>
        [HttpGet]
        [Route("lowest-loudness")]
        [ProducesResponseType(typeof(ApiResponse<TrackWithAudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLowestLoudnessSavedTrack() =>
            await MakeRequest(() => _service.GetUserLowestLoudnessSavedTrack(Tokens));

        //TODO: Add artist that you've been listening to longest (occurs more than once, with longest time span between oldest
        //      and newest occurrences)
    }
}