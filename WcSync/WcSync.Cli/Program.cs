using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tm.WcSync.Db;
using Tm.WcSync.Model;
using Tm.WcSync.Sync;
using Tm.WcSync.Wc;

namespace Tm.WcSync.Cli;
class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) => ConfigureServices(args, context, services));

    private static IServiceCollection ConfigureServices(
        string[] args,
        HostBuilderContext context,
        IServiceCollection services)
    {
        var command = args.Length == 1 ? args[0].ToLower() : "update";

        services
            .AddHostedService<HostedService>(serviceProvider => 
                new HostedService(serviceProvider.GetService<ISyncService>(), command))
            .AddTransient<ISyncService, SyncService>()
            .AddTransient<IWcProductService, WcProductService>()
            .AddTransient<IProductService, ProductService>()
            .AddTransient<IPriceCalculator, PriceCalculator>()
            .AddSingleton<IDbProductRepository, DbProductRepository>();

        return services;
    }
}
