using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InfoController : BaseRequestController
    {
        private readonly ISpotifyService _service;

        public InfoController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        [HttpGet]
        [Route("audio-features/{trackId}")]
        public async Task<IActionResult> GetTrackAudioFeatures(string trackId) =>
            await MakeRequest(() => _service.GetTrackAudioFeatures(Tokens, trackId));
    }
}