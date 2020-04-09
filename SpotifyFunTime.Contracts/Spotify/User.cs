using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class User
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("images")]
        public List<Image> Images { get; set; }
        [JsonProperty("followers")]
        public Followers Followers { get; set; }
    }
}