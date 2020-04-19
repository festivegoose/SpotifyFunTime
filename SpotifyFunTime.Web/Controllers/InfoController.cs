using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/info")]
    [ApiController]
    public class InfoController : BaseRequestController
    {
        private readonly ISpotifyService _service;

        public InfoController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        /// <summary>
        /// Returns an Audio Features object for a given track.
        /// </summary>
        [HttpGet]
        [Route("audio-features/{trackId}")]
        [ProducesResponseType(typeof(ApiResponse<AudioFeatures>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrackAudioFeatures(string trackId) =>
            await MakeRequest(() => _service.GetTrackAudioFeatures(Tokens, trackId));
    }
}