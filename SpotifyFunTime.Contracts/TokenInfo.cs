using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts
{
    public class TokenInfo
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresInSeconds { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}