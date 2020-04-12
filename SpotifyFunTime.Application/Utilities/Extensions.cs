using System.Collections.Generic;
using SpotifyFunTime.Contracts;
using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Application.Utilities
{
    public static class Extensions
    {
        public static ApiResponse<List<T>> ToListContent<T>(this ApiResponse<Paging<T>> response)
        {
            return new ApiResponse<List<T>>(response.StatusCode)
            {
                ReasonPhrase = response.ReasonPhrase,
                Content = response.Content?.Items
            };
        }
    }
}