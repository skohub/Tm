using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WcSync.Model;
using WcSync.Sync;
using WcSync.Wc;

namespace WcSync.Cli;
class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
            )
            .ConfigureServices((context, services) => ConfigureServices(args, context, services));

    private static IServiceCollection ConfigureServices(
        string[] args,
        HostBuilderContext context,
        IServiceCollection services)
    {
        services
            .AddHostedService<HostedService>(services => new HostedService(
                services.GetService<IHostApplicationLifetime>()!,
                services.GetService<ISyncService>()!,
                services.GetService<ILogger>()!))
            .AddTransient<IWcProductService, WcProductService>(services => 
            new WcProductService(
                context.Configuration.GetSection("Wc").Get<WcConfiguration>()!,
                services.GetService<ILogger>()!))
            .AddTransient<ISyncService, SyncService>(services => new SyncService(
                services.GetService<IWcProductService>()!,
                services.GetService<IProductsRepository>()!,
                services.GetService<IPriceCalculator>()!,
                context.Configuration.GetValue<int>(SettingNames.OrganizationId),
                services.GetService<ILogger>()!
            ))
            .AddTransient<IPriceCalculator, PriceCalculator>()
            .AddTransient<IProductsRepository, ProductsRepository>()
            .AddTransient<IConnectionFactory>(services => new MySqlConnectionFactory(
                context.Configuration
                    .GetSection("ConnectionStrings")
                    .Get<Dictionary<string, string>>()!));

        return services;
    }
}
