using Newtonsoft.Json;

namespace SpotifyFunTime.Contracts.Spotify
{
    public class AudioFeatures
    {
        [JsonProperty("duration_ms")]
        public int DurationMs { get; set; }
        public AudioKey Key { get; set; }
        public AudioMode Mode { get; set; }
        [JsonProperty("time_signature")]
        public int TimeSignature { get; set; }
        public float Acousticness { get; set; }
        public float Danceability { get; set; }
        public float Energy { get; set; }
        public float Instrumentalness { get; set; }
        public float Liveness { get; set; }
        public float Loudness { get; set; }
        public float Speechiness { get; set; }
        public float Valence { get; set; }
        public float Tempo { get; set; }
    }

    public enum AudioKey
    {
        Unknown = -1,
        C = 0,
        CSharp,
        D,
        DSharp,
        E,
        F,
        FSharp,
        G,
        GSharp,
        A,
        ASharp,
        B
    }

    public enum AudioMode
    {
        Minor,
        Major
    }
}