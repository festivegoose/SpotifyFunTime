using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : BaseRequestController
    {
        private readonly ISpotifyService _service;

        public UserController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        /// <summary>
        /// Returns the current logged in Spotify User.
        /// </summary>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser() =>
            await MakeRequest(() => _service.GetCurrentUser(Tokens));
    }
}