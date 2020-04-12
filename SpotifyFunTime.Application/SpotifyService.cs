using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyFunTime.Application.Utilities;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;
using System.Net;

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
            await _client.SendAsyncWithCaching<User>(tokenSet, HttpMethod.Get, "me");

        //TODO: Results of this aren't accurate - see GitHub issue https://github.com/spotify/web-api/issues/1441
        public async Task<ApiResponse<List<PlayHistory>>> GetLastTenPlayedTracks(TokenSet tokenSet) =>
            (await _client.SendAsync<Paging<PlayHistory>>(tokenSet, HttpMethod.Get, "me/player/recently-played")).ToListContent();

        public async Task<ApiResponse<List<Track>>> GetUserTopTracks(TokenSet tokenSet, string timeRange, int limit) =>
            (await _client.SendAsyncWithCaching<Paging<Track>>(tokenSet, HttpMethod.Get, $"me/top/tracks?limit={limit}&time_range={timeRange}")).ToListContent();

        public async Task<ApiResponse<List<Artist>>> GetUserTopArtists(TokenSet tokenSet, string timeRange, int limit) =>
            (await _client.SendAsyncWithCaching<Paging<Artist>>(tokenSet, HttpMethod.Get, $"me/top/artists?limit={limit}&time_range={timeRange}")).ToListContent();

        public async Task<ApiResponse<List<SavedTrack>>> GetUserSavedTracks(TokenSet tokenSet, int limit = 50) =>
            await _client.SendAsyncWithPagedCaching<SavedTrack>(tokenSet, HttpMethod.Get, $"me/tracks?limit={limit}");

        public async Task<ApiResponse<List<SavedTrack>>> GetUserMostPopularTracks(TokenSet tokenSet, int limit)
        {
            var response = await GetUserSavedTracks(tokenSet);

            if (response.IsSuccessStatusCode)
            {
                var mostPopularTracks = response.Content.OrderByDescending(x => x.Track.Popularity).Take(limit).ToList();

                return new ApiResponse<List<SavedTrack>>(HttpStatusCode.OK)
                {
                    Content = mostPopularTracks
                };
            }

            return response;
        }

        public async Task<ApiResponse<List<SavedTrack>>> GetUserLeastPopularTracks(TokenSet tokenSet, int limit)
        {
            var response = await GetUserSavedTracks(tokenSet);

            if (response.IsSuccessStatusCode)
            {
                var leastPopularTracks = response.Content.OrderBy(x => x.Track.Popularity).Take(limit).ToList();

                return new ApiResponse<List<SavedTrack>>(HttpStatusCode.OK)
                {
                    Content = leastPopularTracks
                };
            }

            return response;
        }
    }

    public interface ISpotifyService
    {
        Task<ApiResponse<User>> GetCurrentUser(TokenSet tokenSet);
        Task<ApiResponse<List<PlayHistory>>> GetLastTenPlayedTracks(TokenSet tokenSet);
        Task<ApiResponse<List<Track>>> GetUserTopTracks(TokenSet tokenSet, string timeRange, int limit);
        Task<ApiResponse<List<Artist>>> GetUserTopArtists(TokenSet tokenSet, string timeRange, int limit);
        Task<ApiResponse<List<SavedTrack>>> GetUserSavedTracks(TokenSet tokenSet, int limit = 50);
        Task<ApiResponse<List<SavedTrack>>> GetUserMostPopularTracks(TokenSet tokenSet, int limit);
        Task<ApiResponse<List<SavedTrack>>> GetUserLeastPopularTracks(TokenSet tokenSet, int limit);
    }
}