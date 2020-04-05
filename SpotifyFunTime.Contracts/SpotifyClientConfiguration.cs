namespace SpotifyFunTime.Contracts
{
    public class SpotifyClientConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccountBaseUri { get; set; }
        public string ApiBaseUri { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }
}