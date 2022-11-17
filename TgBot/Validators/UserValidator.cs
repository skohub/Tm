using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Tm.TgBot.Validators;
public class UserValidator : IValidator
{
    private ITelegramBotClient _botClient { get; set; }
    private HashSet<long> _allowedUserIds { get; set; }
    private readonly ILogger _logger;

    public UserValidator(
        ITelegramBotClient botClient,
        IEnumerable<long> allowedUserIds,
        ILogger logger)
    {
        _botClient = botClient;
        _allowedUserIds = new HashSet<long>(allowedUserIds);
        _logger = logger;
    }

    public async Task<bool> ValidateAsync(Message message)
    {
        var userId = message.From?.Id;
        if (userId == null || !_allowedUserIds.Contains(userId.Value))
        {
            _logger.Information("User {UserId} not allowed", userId);

            await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Вам здесь не рады.",
                    parseMode: ParseMode.Markdown);

            return false;
        }

        return true;
    }
}