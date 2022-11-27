using Api.Service.Models;
using Api.Service.Services;
using Data;
using Data.Interfaces;

namespace Api.Service
{
    public static class ServiceRegistrationExtensions
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IConnectionFactory>(services =>
                new MySqlConnectionFactory(builder.Configuration
                    .GetSection("ConnectionStrings")
                    .Get<Dictionary<string, string>>()!));
            builder.Services.AddTransient<IArbitrarySqlService, ArbitrarySqlService>();
            builder.Services.AddTransient<IUserService>(services =>
                new UserService(builder.Configuration
                    .GetSection("Users")
                    .Get<User[]>()!));
        }
    }
}