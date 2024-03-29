﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TgBot.Commands;
using TgBot.Commands.Common;
using TgBot.HostedServices;
using TgBot.Validators;
using Data.Interfaces;
using Data.Repositories;
using TgBot.Services;
using Serilog;
using Data;

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
        var configuration = context.Configuration.GetSection("Configuration").Get<Configuration>()!;

        serviceCollection
            .AddHostedService<CommandHostedService>()
            .AddHostedService<NotificationHostedService>(services => 
                new NotificationHostedService(
                    logger: services.GetService<ILogger>()!,
                    messagesRepository: services.GetService<IMessagesRepository>()!,
                    notificationService: services.GetService<INotificationService>()!,
                    pollingInterval: configuration.PollingIntervalMilliseconds
                )
            )
            .AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(configuration.Token))
            .AddTransient<ICommandService, CommandService>()
            .AddTransient<INotificationService, NotificationService>(services =>
                new NotificationService(
                    services.GetService<ITelegramBotClient>()!,
                    configuration.Subscriptions,
                    services.GetService<ILogger>()!
                )
            )
            .AddTransient<ISalesReportsRepository, SalesReportsRepository>()
            .AddTransient<IMessagesRepository, MessagesRepository>()
            .AddTransient<IValidator>(services => 
                new UserValidator(
                    services.GetService<ITelegramBotClient>()!,
                    configuration.AllowedUserIds,
                    services.GetService<ILogger>()!
                ))
            .AddTransient<ICommand, HelpCommand>()
            .AddTransient<ICommand, DailySalesCommand>()
            .AddTransient<ICommand, ProductsTotalAmountCommand>()
            .AddTransient<ICommand, MonthlySalesCommand>()
            .AddTransient<IConnectionFactory>(services => new MySqlConnectionFactory(
                context.Configuration
                    .GetSection("ConnectionStrings")
                    .Get<Dictionary<string, string>>()!));

        return serviceCollection;
    }
}
