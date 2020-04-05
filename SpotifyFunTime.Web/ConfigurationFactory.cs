using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SpotifyFunTime.Web
{
    public class ConfigurationFactory
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.SpotifyClient.json", false)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", true)
                .Build();
        }
    }
}