using System.Net.Http;
using System.Threading.Tasks;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Application
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IClient _client;

        public SpotifyService(IClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<User>> GetCurrentUser(TokenSet tokenSet) =>
            await _client.SendAsync<User>(tokenSet, HttpMethod.Get, "me");

        //TODO: Results of this aren't accurate - see GitHub issue https://github.com/spotify/web-api/issues/1441
        public async Task<ApiResponse<Paging<PlayHistory>>> GetLastTenPlayedTracks(TokenSet tokenSet) =>
            await _client.SendAsync<Paging<PlayHistory>>(tokenSet, HttpMethod.Get, "me/player/recently-played");

        public async Task<ApiResponse<Paging<Track>>> GetTopTracks(TokenSet tokenSet, string timeRange, int limit) =>
            await _client.SendAsync<Paging<Track>>(tokenSet, HttpMethod.Get, $"me/top/tracks?limit={limit}&time_range={timeRange}");

        public async Task<ApiResponse<Paging<Artist>>> GetTopArtists(TokenSet tokenSet, string timeRange, int limit) =>
            await _client.SendAsync<Paging<Artist>>(tokenSet, HttpMethod.Get, $"me/top/artists?limit={limit}&time_range={timeRange}");
    }

    public interface ISpotifyService
    {
        Task<ApiResponse<User>> GetCurrentUser(TokenSet tokenSet);
        Task<ApiResponse<Paging<PlayHistory>>> GetLastTenPlayedTracks(TokenSet tokenSet);
        Task<ApiResponse<Paging<Track>>> GetTopTracks(TokenSet tokenSet, string timeRange, int limit);
        Task<ApiResponse<Paging<Artist>>> GetTopArtists(TokenSet tokenSet, string timeRange, int limit);
    }
}