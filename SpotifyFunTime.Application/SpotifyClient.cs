using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Application
{
    public class SpotifyClient : ISpotifyClient
    {
        private readonly SpotifyClientConfiguration _config;
        private readonly HttpClient _client;

        public SpotifyClient(SpotifyClientConfiguration config, HttpClient client)
        {
            _config = config;
            _client = client;
        }

        public async Task<T> CallSpotify<T>(TokenInfo info, HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage(method, $"{_config.ApiBaseUri}/{uri}")
            {
                Headers =
                {
                    { "Authorization", $"Bearer {info.AccessToken}" }
                }
            };
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }

    public interface ISpotifyClient
    {
        Task<T> CallSpotify<T>(TokenInfo info, HttpMethod method, string uri);
    }
}