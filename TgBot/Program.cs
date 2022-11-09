using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Tm.TgBot.Commands;
using Tm.TgBot.Commands.Common;
using Tm.TgBot.Validators;
using Tm.Data.Interfaces;
using Tm.Data.Repositories;
using Tm.TgBot.Services;

namespace Tm.TgBot;
class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) => ConfigureServices(context, services));

    public static IServiceCollection ConfigureServices(HostBuilderContext context, IServiceCollection serviceCollection) {
        var botConfiguration = context.Configuration.GetSection("Configuration").Get<Configuration>();

        serviceCollection
            .AddHostedService<BotHostedService>()
            .AddTransient<IBotService, BotService>()
            .AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botConfiguration.Token))
            .AddSingleton<ISalesReportsRepository>(serviceProvider => 
                new SalesReportsRepository(context.Configuration.GetConnectionString("mag5")))
            .AddSingleton<IValidator>(serviceProvider => 
                new UserValidator(serviceProvider.GetService<ITelegramBotClient>()!, botConfiguration.AllowedUserIds))
            .AddTransient<ICommand, HelpCommand>()
            .AddTransient<ICommand, DailySalesCommand>()
            .AddTransient<ICommand, ProductsTotalAmountCommand>()
            .AddTransient<ICommand, MonthlySalesCommand>();

        return serviceCollection;
    }
}
