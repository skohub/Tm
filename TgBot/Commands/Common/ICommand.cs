using Telegram.Bot;

namespace TgBot.Commands.Common;
public interface ICommand
{
    string Name { get; }
    Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client);
}
