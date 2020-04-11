using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Application
{
    public class Client : IClient
    {
        private readonly IClientConfiguration _config;
        private readonly HttpClient _client;
        private readonly IContentCache _cache;

        public Client(IClientConfiguration config, HttpClient client, IContentCache cache)
        {
            _config = config;
            _client = client;
            _cache = cache;
        }

        public async Task<ApiResponse<T>> SendAsync<T>(TokenSet tokenSet, HttpMethod method, string endpoint)
        {
            var request = new HttpRequestMessage(method, $"{_config.ApiBaseUri}/{endpoint}")
            {
                Headers =
                {
                    { "Authorization", $"Bearer {tokenSet.AccessToken}" }
                }
            };
            var response = await _client.SendAsync(request);
            var apiResponse = new ApiResponse<T>(response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                apiResponse.Content = JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                apiResponse.ReasonPhrase = response.ReasonPhrase;
            }

            return apiResponse;
        }

        public async Task<ApiResponse<T>> SendAsyncWithCaching<T>(TokenSet tokenSet, HttpMethod method, string endpoint)
        {
            var requestUrl = $"{_config.ApiBaseUri}/{endpoint}";
            var cacheKey = $"{requestUrl}_{tokenSet.AccessToken}";

            if (_cache.TryGet(cacheKey, out T content))
            {
                return new ApiResponse<T>(HttpStatusCode.OK)
                {
                    Content = content
                };
            }

            var response = await SendAsync<T>(tokenSet, method, endpoint);

            if (response.IsSuccessStatusCode)
            {
                _cache.Set(cacheKey, response.Content);
            }

            return response;
        }
    }

    public interface IClient
    {
        Task<ApiResponse<T>> SendAsync<T>(TokenSet tokenSet, HttpMethod method, string endpoint);
        Task<ApiResponse<T>> SendAsyncWithCaching<T>(TokenSet tokenSet, HttpMethod method, string endpoint);
    }
}