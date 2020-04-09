using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Application
{
    public class SpotifyAuthClient : ISpotifyAuthClient
    {
        private readonly IClientConfiguration _config;
        private readonly HttpClient _client;
        
        public SpotifyAuthClient(IClientConfiguration config, HttpClient client)
        {
            _config = config;
            _client = client;
        }
        
        public async Task<TokenSet> GetToken(string code)
        {
            var request = BuildRequest(code);
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenSet>(content);

            return tokenResponse;
        }

        public async Task<TokenSet> RefreshToken(string refreshToken)
        {
            var request = BuildRefreshRequest(refreshToken);
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenSet>(content);

            return tokenResponse;
        }

        private HttpRequestMessage BuildRequest(string code)
        {
            var body = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", _config.RedirectUri}
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.AccountBaseUri}/api/token")
            {
                Headers =
                {
                    { "Authorization", $"Basic {GetBasicAuthToken()}" }
                },
                Content =  new FormUrlEncodedContent(body)
            };

            return request;
        }

        private HttpRequestMessage BuildRefreshRequest(string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.AccountBaseUri}/api/token")
            {
                Headers =
                {
                    { "Authorization", $"Basic {GetBasicAuthToken()}" }
                },
                Content =  new FormUrlEncodedContent(body)
            };

            return request;
        }

        private string GetBasicAuthToken()
        {
            var bytes = Encoding.UTF8.GetBytes($"{_config.ClientId}:{_config.ClientSecret}");
            return Convert.ToBase64String(bytes);
        }
    }

    public interface ISpotifyAuthClient
    {
        Task<TokenSet> GetToken(string code);
        Task<TokenSet> RefreshToken(string refreshToken);
    }
}