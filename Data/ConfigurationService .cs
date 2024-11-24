using Microsoft.Extensions.Configuration;
using System.IO;

namespace Data
{
    public static class ConfigurationService
    {
        public static IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path for configuration (appsettings.json)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);  // Add the appsettings.json file

            return configurationBuilder.Build();
        }
    }
}
