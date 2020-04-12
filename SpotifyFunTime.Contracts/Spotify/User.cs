using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class User
    {
        public string Id { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        public List<Image> Images { get; set; }
        public Followers Followers { get; set; }
    }
}