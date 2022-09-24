using Telegram.Bot;
using Tm.TgBot.Commands.Common;

namespace Tm.TgBot.Commands;

public class HelpCommand : ICommand
{
    public Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        return Task.FromResult(
            new CommandResult
            {
                Text = $"Не понял команду \"{arguments?.FirstOrDefault()}\". Попробуйте написать \"Продажи\".",
            });
    }
}
