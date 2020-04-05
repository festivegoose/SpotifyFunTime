using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web.Controllers
{
    public class BaseController : Controller
    {
        protected TokenSet Tokens =>
            new TokenSet
            {
                AccessToken = HttpContext.Session.GetString("_accessToken"),
                RefreshToken = HttpContext.Session.GetString("_refreshToken")
            };
        
        protected void SetSessionInfo(TokenSet tokenSet)
        {
            HttpContext.Session.SetString("_accessToken", tokenSet.AccessToken);

            if (!string.IsNullOrWhiteSpace(tokenSet.RefreshToken))
            {
                HttpContext.Session.SetString("_refreshToken", tokenSet.RefreshToken);   
            }
        }
    }
}