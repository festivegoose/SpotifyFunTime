using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : BaseRequestController
    {
        private readonly ISpotifyService _service;

        public UserController(ISpotifyAuthClient authClient, ISpotifyService service) : base(authClient)
        {
            _service = service;
        }

        [HttpGet]
        [Route("welcome")]
        public async Task<IActionResult> Welcome() =>
            await MakeRequest(() => _service.GetCurrentUser(Tokens));
    }
}