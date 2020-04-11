using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

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
            var requestUrl = endpoint.Contains("https://") ? endpoint : $"{_config.ApiBaseUri}/{endpoint}";
            var request = new HttpRequestMessage(method, requestUrl)
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
            var cacheKey = $"{endpoint}_{tokenSet.AccessToken}";

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

        public async Task<ApiResponse<List<T>>> SendAsyncWithPagedCaching<T>(TokenSet tokenSet, HttpMethod method, string endpoint)
        {
            var endpointUrlWithoutParams = endpoint.Split('?')[0];
            var cacheKey = $"{endpointUrlWithoutParams}_paged_{tokenSet.AccessToken}";
            var itemCollection = new List<T>();

            if (_cache.TryGet(cacheKey, out List<T> content))
            {
                return new ApiResponse<List<T>>(HttpStatusCode.OK)
                {
                    Content = content
                };
            }

            var response = new ApiResponse<Paging<T>>(HttpStatusCode.OK);

            while(itemCollection.Count == 0 || !string.IsNullOrWhiteSpace(response.Content.Next))
            {
                response = await SendAsync<Paging<T>>(tokenSet, method, endpoint);

                if (response.IsSuccessStatusCode)
                {
                    itemCollection.AddRange(response.Content.Items);
                    endpoint = response.Content.Next;
                }
            }

            var completedResponse = new ApiResponse<List<T>>(HttpStatusCode.OK)
            {
                Content = itemCollection
            };

            _cache.Set(cacheKey, completedResponse.Content);

            return completedResponse;
        }
    }

    public interface IClient
    {
        Task<ApiResponse<T>> SendAsync<T>(TokenSet tokenSet, HttpMethod method, string endpoint);
        Task<ApiResponse<T>> SendAsyncWithCaching<T>(TokenSet tokenSet, HttpMethod method, string endpoint);
        Task<ApiResponse<List<T>>> SendAsyncWithPagedCaching<T>(TokenSet tokenSet, HttpMethod method, string endpoint);
    }
}