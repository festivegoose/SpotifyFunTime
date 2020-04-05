using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly SpotifyClientConfiguration _config;
        private readonly ISpotifyAuthClient _authClient;

        public AuthorizationController(SpotifyClientConfiguration config, ISpotifyAuthClient authClient)
        {
            _config = config;
            _authClient = authClient;
        }

        [Route("authorize")]
        public IActionResult Index()
        {
            var authParams = new Dictionary<string, string>
            {
                {"client_id", _config.ClientId},
                {"response_type", "code"},
                {"redirect_uri", _config.RedirectUri},
                {"scope", _config.Scope}
            };

            var redirectUrl = QueryHelpers.AddQueryString($"{_config.AccountBaseUri}/authorize", authParams);
            
            return Redirect(redirectUrl);
        }

        [Route("sign-in")]
        public async Task<IActionResult> SignIn(string code)
        {
            var token = await _authClient.GetToken(code);
            
            SetSessionInfo(token);

            return Redirect("/welcome");
        }

        private void SetSessionInfo(TokenInfo info)
        {
            HttpContext.Session.SetString("_accessToken", info.AccessToken);
            HttpContext.Session.SetString("_refreshToken", info.RefreshToken);
        }
    }
}