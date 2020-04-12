using System.Collections.Generic;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class Track
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Artist> Artists { get; set; }
        public int Popularity { get; set; }
    }
}