using System;
using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class PlayHistory
    {
        public Track Track { get; set; }
        [JsonProperty("played_at")]
        public DateTime PlayedAt { get; set; }
    }
}