using System.Net.Http;
using System.Threading.Tasks;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Application
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IClient _client;

        public SpotifyService(IClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<SpotifyUser>> GetCurrentUser(TokenSet tokenSet)
        {
            var userResponse = await _client.SendAsync<SpotifyUser>(tokenSet, HttpMethod.Get, "me");

            return userResponse;
        }
    }

    public interface ISpotifyService
    {
        Task<ApiResponse<SpotifyUser>> GetCurrentUser(TokenSet tokenSet);
    }
}