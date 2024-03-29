using Telegram.Bot.Types.ReplyMarkups;

namespace TgBot.Commands.Common;
public class CommandResult
{
    public string Text { get; set; } = string.Empty;
    public IReplyMarkup? ReplyMarkup { get; set; }
}