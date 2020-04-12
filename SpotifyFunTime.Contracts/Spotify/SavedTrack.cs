using System;
using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class SavedTrack
    {
        [JsonProperty("added_at")]
        public DateTime AddedAt { get; set;}
        public Track Track { get; set; }
    }
}