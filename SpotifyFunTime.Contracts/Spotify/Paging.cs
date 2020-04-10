using System.Collections.Generic;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class Paging<T>
    {
        public List<T> Items { get; set;}
        public string Next { get; set; }
    }
}