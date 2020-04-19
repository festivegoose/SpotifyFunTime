using SpotifyFunTime.Contracts.Spotify;

namespace SpotifyFunTime.Contracts.Custom
{
    public class TrackWithAudioFeatures : Track
    {
        public AudioFeatures AudioFeatures { get; set; }

        public TrackWithAudioFeatures() { }

        public TrackWithAudioFeatures(Track track)
        {
            Id = track.Id;
            Name = track.Name;
            Artists = track.Artists;
            Popularity = track.Popularity;
        }
    }
}