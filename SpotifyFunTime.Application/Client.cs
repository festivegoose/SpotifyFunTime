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

        public Client(IClientConfiguration config, HttpClient client)
        {
            _config = config;
            _client = client;
        }

        public async Task<ApiResponse<T>> SendAsync<T>(TokenSet tokenSet, HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage(method, $"{_config.ApiBaseUri}/{uri}")
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
    }

    public interface IClient
    {
        Task<ApiResponse<T>> SendAsync<T>(TokenSet tokenSet, HttpMethod method, string uri);
    }
}