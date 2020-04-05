using System.Net.Http;
using System.Threading.Tasks;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Application
{
    public class SpotifyService : ISpotifyService
    {
        private readonly ISpotifyClient _client;

        public SpotifyService(ISpotifyClient client)
        {
            _client = client;
        }

        public async Task<SpotifyUser> GetCurrentUser(TokenInfo info)
        {
            var user = await _client.CallSpotify<SpotifyUser>(info, HttpMethod.Get, "me");

            return user;
        }
    }

    public interface ISpotifyService
    {
        Task<SpotifyUser> GetCurrentUser(TokenInfo info);
    }
}