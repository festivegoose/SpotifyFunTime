using System.Collections.Generic;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class Artist
    {
        public string Name { get; set; }
        public Followers Followers { get; set; }
        public List<string> Genres { get; set; }
        public int Popularity { get; set; }
        public List<Image> Images { get; set; }
    }
}