using System.Globalization;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tm.TgBot.Commands.Common;
using Tm.TgBot.Services;
using Tm.TgBot.Validators;

namespace Tm.TgBot;
public class BotHostedService : IHostedService
{
    private ITelegramBotClient _botClient;
    private IEnumerable<IValidator> _validators;
    private IBotService _botService;

    public BotHostedService(
        ITelegramBotClient botClient, 
        IEnumerable<IValidator> validators, 
        IBotService botService)
    {
        _botClient = botClient;
        _validators = validators;
        _botService = botService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        CultureInfo.CurrentCulture = new CultureInfo("ru-RU", false);

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken);

        var me = await _botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stop");

        return Task.CompletedTask;
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

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}. UserId: {userId}.");

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
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
