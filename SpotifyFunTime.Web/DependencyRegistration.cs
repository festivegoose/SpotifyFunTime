using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyFunTime.Application;
using SpotifyFunTime.Contracts;

namespace SpotifyFunTime.Web
{
    public static class DependencyRegistration
    {
        public static void Register(IConfiguration configuration, IServiceCollection services)
        {
            var spotifyClientConfiguration = new SpotifyClientConfiguration();
            configuration.GetSection("SpotifySettings").Bind(spotifyClientConfiguration);
            configuration.GetSection("SpotifyClientConfiguration").Bind(spotifyClientConfiguration);

            services.AddSingleton<IClientConfiguration>(spotifyClientConfiguration);

            var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5)};
            services.AddSingleton(httpClient);

            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IContentCache, ContentCache>();

            services.AddScoped<ISpotifyAuthClient, SpotifyAuthClient>();
            services.AddScoped<IClient, Client>();
            services.AddScoped<ISpotifyService, SpotifyService>();
        }
    }
}