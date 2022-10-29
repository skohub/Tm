using Microsoft.Extensions.Configuration;

namespace Api.Tests
{
    public static class Configuration
    {
        private static IConfiguration? _configuration;

        public static IConfiguration GetConfiguration()
        {
            if (_configuration != null) return _configuration;

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", true)
                .Build();

            return _configuration;
        }
    }
}