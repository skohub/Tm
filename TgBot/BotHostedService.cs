using System.Globalization;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBot.Services;
using TgBot.Validators;

namespace TgBot;
public class BotHostedService : BackgroundService
{
    private ITelegramBotClient _botClient;
    private IEnumerable<IValidator> _validators;
    private IBotService _botService;
    private readonly ILogger _logger;

    public BotHostedService(
        ITelegramBotClient botClient, 
        IEnumerable<IValidator> validators, 
        IBotService botService,
        ILogger logger)
    {
        _botClient = botClient;
        _validators = validators;
        _botService = botService;
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CultureInfo.CurrentCulture = new CultureInfo("ru-RU", false);
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };

        await _botClient.ReceiveAsync(HandleUpdateAsync, HandleErrorAsync, receiverOptions, stoppingToken);
    }

    public async override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Stop");

        await base.StopAsync(cancellationToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        {
            return;
        };

        var messageText = update.Message.Text!;
        var chatId = update.Message.Chat.Id;
        var userId = update.Message.From?.Id;

        _logger.Information("Received a '{MessageText}' message in chat {ChatId}. UserId: {UserId}.",
            messageText, chatId, userId);

        var validationResults = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(update.Message)));
        if (validationResults.Any(x => x == false))
        {
            return;
        }

        await _botService.HandleMessageAsync(botClient, update.Message, cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => 
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        _logger.Error(exception, ErrorMessage);

        return Task.CompletedTask;
    }
}
