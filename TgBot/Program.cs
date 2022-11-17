using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TgBot.Commands;
using TgBot.Commands.Common;
using TgBot.Validators;
using Data.Interfaces;
using Data.Repositories;
using TgBot.Services;
using Serilog;

namespace TgBot;
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
            .ConfigureServices((context, services) => ConfigureServices(context, services));

    public static IServiceCollection ConfigureServices(HostBuilderContext context, IServiceCollection serviceCollection) {
        var botConfiguration = context.Configuration.GetSection("Configuration").Get<Configuration>();

        serviceCollection
            .AddHostedService<BotHostedService>()
            .AddTransient<IBotService, BotService>()
            .AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botConfiguration.Token))
            .AddSingleton<ISalesReportsRepository>(services => 
                new SalesReportsRepository(context.Configuration.GetConnectionString("mag5")))
            .AddSingleton<IValidator>(services => 
                new UserValidator(services.GetService<ITelegramBotClient>()!,
                botConfiguration.AllowedUserIds,
                services.GetService<ILogger>()!))
            .AddTransient<ICommand, HelpCommand>()
            .AddTransient<ICommand, DailySalesCommand>()
            .AddTransient<ICommand, ProductsTotalAmountCommand>()
            .AddTransient<ICommand, MonthlySalesCommand>();

        return serviceCollection;
    }
}
