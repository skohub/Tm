using System.Text.RegularExpressions;
using Data.Models.Messasges;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TgBot.Models;

namespace TgBot.Services;
public class NotificationService : INotificationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IEnumerable<NotificationSubscription> _subscriptions;
    private readonly ILogger _logger;

    public NotificationService(
        ITelegramBotClient botClient,
        IEnumerable<NotificationSubscription> subscriptions,
        ILogger logger)
    {
        _botClient = botClient;
        _subscriptions = subscriptions;
        _logger = logger;
    }

    public async Task SendAsync(IEnumerable<Message> messages)
    {
        await Task.WhenAll(messages
            .SelectMany(x => GetMessageSubscriptions(x).Select(y => (message: x, subscription: y)))
            .Select(x => SendMessageAsync(x.message, x.subscription)));
    }

    private IEnumerable<NotificationSubscription> GetMessageSubscriptions(Message message) =>
        _subscriptions
            .Where(x => string.Equals(x.Type, message.Type, StringComparison.InvariantCultureIgnoreCase))
            .Where(x => Regex.IsMatch(message.User, x.UserPattern));

    private string PrepareMessageText(Message message, NotificationSubscription subscription) => 
        subscription.NotificationTemplate
            .Replace("@msg", message.Msg)
            .Replace("@user", message.User)
            .Replace("@date", message.Date.ToString("dd.MM.yy HH:mm"));

    private async Task SendMessageAsync(Message message, NotificationSubscription subscription)
    {
        var text = PrepareMessageText(message, subscription);
        await _botClient.SendTextMessageAsync(
            chatId: subscription.ChatId,
            text: text,
            parseMode: ParseMode.Markdown);

        _logger.Information("Sent message {Text} to chat {ChatId}", text, subscription.ChatId);
    }
}