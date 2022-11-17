using Telegram.Bot;
using TgBot.Commands.Common;

namespace TgBot.Commands;

public class HelpCommand : ICommand
{
    public string Name => "/help";

    public Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        return Task.FromResult(
            new CommandResult
            {
                Text = $"Не понял команду \"{arguments?.FirstOrDefault()}\", попробуйте написать /dailysales.",
            });
    }
}
