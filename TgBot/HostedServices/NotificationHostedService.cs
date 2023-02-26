using Data.Models.Meessages;
using Data.Repositories;
using Microsoft.Extensions.Hosting;
using Serilog;
using TgBot.Services;

namespace TgBot.HostedServices;
public class NotificationHostedService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IMessagesRepository _messagesRepository;
    private readonly INotificationService _notificationService;
    private readonly int _pollingInterval;

    public NotificationHostedService(
        ILogger logger, 
        IMessagesRepository messagesRepository,
        INotificationService notificationService,
        int pollingInterval)
    {
        _logger = logger;
        _messagesRepository = messagesRepository;
        _notificationService = notificationService;
        _pollingInterval = pollingInterval;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Information($"{nameof(NotificationHostedService)} Execute");

        while (true)
        {
            var messages = await _messagesRepository.GetNewMessagesAsync();
            try 
            {
                await _notificationService.SendAsync(messages);
            }
            finally
            {
                await _messagesRepository.SetMessagesStatusAsync(messages, MessageStatus.Complete);
            }

            await Task.Delay(_pollingInterval, stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();
        }
    }

    public async override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information($"{nameof(NotificationHostedService)} Stop");

        await base.StopAsync(cancellationToken);
    }
}