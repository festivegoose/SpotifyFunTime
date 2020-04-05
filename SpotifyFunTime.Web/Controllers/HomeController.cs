using Microsoft.AspNetCore.Mvc;

namespace SpotifyFunTime.Web.Controllers
{
    [Route("")]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            var redirectUrl = string.IsNullOrWhiteSpace(Tokens.AccessToken) ? "/login" : "/welcome";
            
            return Redirect(redirectUrl);
        }
    }
}