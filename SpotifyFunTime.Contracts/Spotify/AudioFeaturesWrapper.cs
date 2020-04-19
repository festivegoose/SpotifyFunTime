using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class AudioFeaturesWrapper
    {
        [JsonProperty("audio_features")]
        public List<AudioFeatures> AudioFeatures { get; set; }
    }
}