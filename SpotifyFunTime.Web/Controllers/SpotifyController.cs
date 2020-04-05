using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SpotifyController : Controller
    {
        private readonly ISpotifyService _service;
        
        private TokenInfo Info =>
            new TokenInfo
            {
                AccessToken = HttpContext.Session.GetString("_accessToken"),
                RefreshToken = HttpContext.Session.GetString("_refreshToken")
            };

        public SpotifyController(ISpotifyService service)
        {
            _service = service;
        }

        [Route("welcome")]
        public async Task<IActionResult> Welcome()
        {
            var currentUser = await _service.GetCurrentUser(Info);
            
            return Ok(currentUser);
        }
    }
}