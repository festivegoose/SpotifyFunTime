using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts
{
    public class SpotifyUser
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
    }
}