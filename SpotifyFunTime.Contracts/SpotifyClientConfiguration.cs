namespace SpotifyFunTime.Contracts
{
    public class SpotifyClientConfiguration : IClientConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccountBaseUri { get; set; }
        public string ApiBaseUri { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }

    public interface IClientConfiguration
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string AccountBaseUri { get; set; }
        string ApiBaseUri { get; set; }
        string RedirectUri { get; set; }
        string Scope { get; set; }
    }
}