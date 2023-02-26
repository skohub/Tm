using System.Globalization;
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
        foreach ((var message, var subscriptions) in GetMessagesToSend(messages))
        {
            foreach (var subscription in subscriptions)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: subscription.ChatId,
                    text: PrepareChatMessage(message, subscription),
                    parseMode: ParseMode.Markdown);
            }
        }
    }

    private IEnumerable<(Message, IEnumerable<NotificationSubscription>)> GetMessagesToSend(
        IEnumerable<Message> messages)
    {
        return messages
            .Select(x => (
                message: x,
                subscriptions: _subscriptions
                    .Where(y => string.Equals(y.Type, x.Type, StringComparison.InvariantCultureIgnoreCase))
                    .Where(y => Regex.IsMatch(x.User, y.UserPattern))
            ));
    }

    private string PrepareChatMessage(Message message, NotificationSubscription subscription) => 
        subscription.NotificationTemplate
            .Replace("@msg", message.Msg)
            .Replace("@user", message.User)
            .Replace("@date", message.Date.ToString("g", new CultureInfo("ru-RU")));
}