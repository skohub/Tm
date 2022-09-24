using Telegram.Bot;

namespace Tm.TgBot.Commands.Common;
public interface ICommand
{
    Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client);
}
