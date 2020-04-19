using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyFunTime.Application.Utilities;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;
using System.Net;
using System;
using SpotifyFunTime.Contracts.Custom;

namespace SpotifyFunTime.Application
{
    public class SpotifyService : ISpotifyService
    {
        private const int MAX_ARTIST_LIMIT_VALUE = 50;
        private const int MAX_FEATURE_LIMIT_VALUE = 100;
        private readonly IClient _client;

        public SpotifyService(IClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<User>> GetCurrentUser(TokenSet tokenSet) =>
            await _client.SendAsyncWithCaching<User>(tokenSet, HttpMethod.Get, "me");

        public async Task<ApiResponse<AudioFeatures>> GetAudioFeaturesForTrack(TokenSet tokenSet, string trackId) =>
            await _client.SendAsync<AudioFeatures>(tokenSet, HttpMethod.Get, $"audio-features/{trackId}");

        public async Task<ApiResponse<List<AudioFeatures>>> GetAudioFeaturesForTracks(TokenSet tokenSet, List<string> trackIds, bool shouldCache = false)
        {
            var featuresList = new List<AudioFeatures>();
            var completedCount = 0;

            while(completedCount != trackIds.Count)
            {
                var currentSet = trackIds.Skip(completedCount).Take(MAX_FEATURE_LIMIT_VALUE);
                var idString = string.Join(",", currentSet);
                var featuresResponse = shouldCache ?
                    await _client.SendAsyncWithCaching<AudioFeaturesWrapper>(tokenSet, HttpMethod.Get, $"audio-features?ids={idString}") :
                    await _client.SendAsync<AudioFeaturesWrapper>(tokenSet, HttpMethod.Get, $"audio-features?ids={idString}");

                if (!featuresResponse.IsSuccessStatusCode)
                {
                    break;
                }
                
                featuresList.AddRange(featuresResponse.Content.AudioFeatures);
                completedCount += featuresResponse.Content.AudioFeatures.Count;
            }

            return new ApiResponse<List<AudioFeatures>>(HttpStatusCode.OK)
            {
                Content = featuresList
            };
        }

        //TODO: Results of this aren't accurate - see GitHub issue https://github.com/spotify/web-api/issues/1441
        public async Task<ApiResponse<List<PlayHistory>>> GetLastTenPlayedTracks(TokenSet tokenSet) =>
            (await _client.SendAsync<Paging<PlayHistory>>(tokenSet, HttpMethod.Get, "me/player/recently-played")).ToListContent();

        public async Task<ApiResponse<List<Track>>> GetUserTopTracks(TokenSet tokenSet, string timeRange, int limit) =>
            (await _client.SendAsyncWithCaching<Paging<Track>>(tokenSet, HttpMethod.Get, $"me/top/tracks?limit={limit}&time_range={timeRange}")).ToListContent();

        public async Task<ApiResponse<List<Artist>>> GetUserTopArtists(TokenSet tokenSet, string timeRange, int limit) =>
            (await _client.SendAsyncWithCaching<Paging<Artist>>(tokenSet, HttpMethod.Get, $"me/top/artists?limit={limit}&time_range={timeRange}")).ToListContent();

        public async Task<ApiResponse<List<SavedTrack>>> GetUserSavedTracks(TokenSet tokenSet, int limit = 50) =>
            await _client.SendAsyncWithPagedCaching<SavedTrack>(tokenSet, HttpMethod.Get, $"me/tracks?limit={limit}");

        public async Task<ApiResponse<List<Artist>>> GetArtists(TokenSet tokenSet, List<string> artistIds, bool shouldCache = false)
        {
            var artistList = new List<Artist>();
            var completedCount = 0;

            while(completedCount != artistIds.Count)
            {
                var currentSet = artistIds.Skip(completedCount).Take(MAX_ARTIST_LIMIT_VALUE);
                var idString = string.Join(",", currentSet);
                var artistsResponse = shouldCache ?
                    await _client.SendAsyncWithCaching<ArtistsWrapper>(tokenSet, HttpMethod.Get, $"artists?ids={idString}") :
                    await _client.SendAsync<ArtistsWrapper>(tokenSet, HttpMethod.Get, $"artists?ids={idString}");

                if (!artistsResponse.IsSuccessStatusCode)
                {
                    break;
                }
                
                artistList.AddRange(artistsResponse.Content.Artists);
                completedCount += artistsResponse.Content.Artists.Count;
            }

            return new ApiResponse<List<Artist>>(HttpStatusCode.OK)
            {
                Content = artistList
            };
        }

        public async Task<ApiResponse<List<SavedTrack>>> GetUserMostPopularSavedTracks(TokenSet tokenSet, int limit)
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

        public async Task<ApiResponse<List<SavedTrack>>> GetUserLeastPopularSavedTracks(TokenSet tokenSet, int limit)
        {
            var response = await GetUserSavedTracks(tokenSet);

            if (response.IsSuccessStatusCode)
            {
                var unpopularTracks = response.Content.Where(x => x.Track.Popularity == 0);

                if (unpopularTracks.Count() < limit)
                {
                    unpopularTracks = response.Content.OrderBy(x => x.Track.Popularity).Take(limit);
                }

                var unpopularArtistIds = unpopularTracks.SelectMany(x => x.Track.Artists).Select(x => x.Id).Distinct().ToList();
                var unpopularArtists = (await GetArtists(tokenSet, unpopularArtistIds)).Content.ToDictionary(x => x.Id);

                unpopularTracks.ToList().ForEach(savedTrack => {
                    savedTrack.Track.Artists.ForEach(artist => {
                        artist.Popularity = unpopularArtists[artist.Id].Popularity;
                    });
                });

                var leastPopularTracks = unpopularTracks.OrderBy(x => x.Track.Popularity).ThenBy(x => x.Track.Artists.Sum(a => a.Popularity)).Take(limit).ToList();
                

                return new ApiResponse<List<SavedTrack>>(HttpStatusCode.OK)
                {
                    Content = leastPopularTracks
                };
            }

            return response;
        }

        public async Task<ApiResponse<List<Artist>>> GetUserMostPopularSavedArtists(TokenSet tokenSet, int limit)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var savedArtistIds = savedTrackResponse.Content.SelectMany(x => x.Track.Artists).Select(x => x.Id).Distinct().ToList();
                var savedArtistsResponse = await GetArtists(tokenSet, savedArtistIds, true);

                if (savedArtistsResponse.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<Artist>>(HttpStatusCode.OK)
                    {
                        Content = savedArtistsResponse.Content.OrderByDescending(x => x.Popularity).Take(limit).ToList()
                    };
                }

                return savedArtistsResponse;
            }

            return new ApiResponse<List<Artist>>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<List<Artist>>> GetUserLeastPopularSavedArtists(TokenSet tokenSet, int limit)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var savedArtistIds = savedTrackResponse.Content.SelectMany(x => x.Track.Artists).Select(x => x.Id).Distinct().ToList();
                var savedArtistsResponse = await GetArtists(tokenSet, savedArtistIds, true);

                if (savedArtistsResponse.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<Artist>>(HttpStatusCode.OK)
                    {
                        Content = savedArtistsResponse.Content.OrderBy(x => x.Popularity).Take(limit).ToList()
                    };
                }

                return savedArtistsResponse;
            }

            return new ApiResponse<List<Artist>>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<Dictionary<string, int>>> GetUserTopGenres(TokenSet tokenSet, string timeRange, int limit)
        {
            var topArtistResponse = await GetUserTopArtists(tokenSet, timeRange, MAX_ARTIST_LIMIT_VALUE);

            if (topArtistResponse.IsSuccessStatusCode)
            {
                var genres = topArtistResponse.Content.SelectMany(x => x.Genres);
                var topGenres = genres.GroupBy(x => x).OrderByDescending(g => g.Count()).Take(limit).ToDictionary(g => g.Key, g => g.Count());

                return new ApiResponse<Dictionary<string, int>>(HttpStatusCode.OK)
                {
                    Content = topGenres
                };
            }

            return new ApiResponse<Dictionary<string, int>>(topArtistResponse.StatusCode)
            {
                ReasonPhrase = topArtistResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<Dictionary<string, int>>> GetUserSavedTracksByMonth(TokenSet tokenSet)
        {
            var savedTracksResponse = await GetUserSavedTracks(tokenSet);

            if (savedTracksResponse.IsSuccessStatusCode)
            {
                var lastYear = DateTime.UtcNow.AddYears(-1).Year;
                var nextMonth = DateTime.UtcNow.AddMonths(1).Month;
                var fromDate = new DateTime(lastYear, nextMonth, 1);
                var savedTracksFromLastYear = savedTracksResponse.Content.Where(x => x.AddedAt >= fromDate);
                var trackGroupings = savedTracksFromLastYear.GroupBy(x => $"{x.AddedAt.Year}-{x.AddedAt.Month.ToString("00")}").OrderBy(g => g.Key);
                var stats = trackGroupings.ToDictionary(g => g.Key, g => g.Count());

                return new ApiResponse<Dictionary<string, int>>(HttpStatusCode.OK)
                {
                    Content = stats
                };
            }

            return new ApiResponse<Dictionary<string, int>>(savedTracksResponse.StatusCode)
            {
                ReasonPhrase = savedTracksResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserMostDanceableSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var mostDanceableTrack = trackList.OrderByDescending(x => x.AudioFeatures.Danceability).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = mostDanceableTrack
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserLeastDanceableSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var leastDanceableTrack = trackList.OrderBy(x => x.AudioFeatures.Danceability).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = leastDanceableTrack
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserMostEnergeticSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var mostEnergeticSong = trackList.OrderByDescending(x => x.AudioFeatures.Energy).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = mostEnergeticSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserLeastEnergeticSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var leastEnergeticSong = trackList.OrderBy(x => x.AudioFeatures.Energy).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = leastEnergeticSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserFastestTempoSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var fastestTempoSong = trackList.OrderByDescending(x => x.AudioFeatures.Tempo).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = fastestTempoSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserSlowestTempoSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var slowestTempoSong = trackList.OrderBy(x => x.AudioFeatures.Tempo).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = slowestTempoSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserHighestValenceSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var highestValenceSong = trackList.OrderByDescending(x => x.AudioFeatures.Valence).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = highestValenceSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserLowestValenceSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var lowestValenceSong = trackList.OrderBy(x => x.AudioFeatures.Valence).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = lowestValenceSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserHighestLoudnessSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var highestLoudnessSong = trackList.OrderByDescending(x => x.AudioFeatures.Loudness).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = highestLoudnessSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }

        public async Task<ApiResponse<TrackWithAudioFeatures>> GetUserLowestLoudnessSavedTrack(TokenSet tokenSet)
        {
            var savedTrackResponse = await GetUserSavedTracks(tokenSet);

            if (savedTrackResponse.IsSuccessStatusCode)
            {
                var trackList = savedTrackResponse.Content.Select(x => new TrackWithAudioFeatures(x.Track)).ToList();
                var savedTrackIds = savedTrackResponse.Content.Select(x => x.Track.Id).Distinct().ToList();
                var audioFeatures = (await GetAudioFeaturesForTracks(tokenSet, savedTrackIds, true)).Content.ToDictionary(x => x.TrackId);

                trackList.ForEach(track => {
                    track.AudioFeatures = audioFeatures[track.Id];
                });

                var lowestLoudnessSong = trackList.OrderBy(x => x.AudioFeatures.Loudness).First();

                return new ApiResponse<TrackWithAudioFeatures>(HttpStatusCode.OK)
                {
                    Content = lowestLoudnessSong
                };
            }

            return new ApiResponse<TrackWithAudioFeatures>(savedTrackResponse.StatusCode)
            {
                ReasonPhrase = savedTrackResponse.ReasonPhrase
            };
        }
    }

    public interface ISpotifyService
    {
        Task<ApiResponse<User>> GetCurrentUser(TokenSet tokenSet);
        Task<ApiResponse<AudioFeatures>> GetAudioFeaturesForTrack(TokenSet tokenSet, string trackId);
        Task<ApiResponse<List<AudioFeatures>>> GetAudioFeaturesForTracks(TokenSet tokenSet, List<string> trackIds, bool shouldCache = false);
        Task<ApiResponse<List<PlayHistory>>> GetLastTenPlayedTracks(TokenSet tokenSet);
        Task<ApiResponse<List<Track>>> GetUserTopTracks(TokenSet tokenSet, string timeRange, int limit);
        Task<ApiResponse<List<Artist>>> GetUserTopArtists(TokenSet tokenSet, string timeRange, int limit);
        Task<ApiResponse<List<SavedTrack>>> GetUserSavedTracks(TokenSet tokenSet, int limit = 50);
        Task<ApiResponse<List<SavedTrack>>> GetUserMostPopularSavedTracks(TokenSet tokenSet, int limit);
        Task<ApiResponse<List<SavedTrack>>> GetUserLeastPopularSavedTracks(TokenSet tokenSet, int limit);
        Task<ApiResponse<Dictionary<string, int>>> GetUserTopGenres(TokenSet tokenSet, string timeRange, int limit);
        Task<ApiResponse<Dictionary<string, int>>> GetUserSavedTracksByMonth(TokenSet tokenSet);
        Task<ApiResponse<List<Artist>>> GetUserMostPopularSavedArtists(TokenSet tokenSet, int limit);
        Task<ApiResponse<List<Artist>>> GetUserLeastPopularSavedArtists(TokenSet tokenSet, int limit);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserMostDanceableSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserLeastDanceableSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserMostEnergeticSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserLeastEnergeticSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserFastestTempoSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserSlowestTempoSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserHighestValenceSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserLowestValenceSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserHighestLoudnessSavedTrack(TokenSet tokenSet);
        Task<ApiResponse<TrackWithAudioFeatures>> GetUserLowestLoudnessSavedTrack(TokenSet tokenSet);
    }
}