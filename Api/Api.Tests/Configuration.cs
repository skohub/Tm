using System.Reflection;
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
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            return _configuration;
        }
    }
}