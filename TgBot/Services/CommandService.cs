using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBot.Commands.Common;

namespace TgBot.Services;
public class CommandService : ICommandService
{
    private IEnumerable<ICommand> _commands;
    private IDictionary<long, ICommand> _state = new Dictionary<long, ICommand>();
    private string _defaultCommandName = "/help";

    public CommandService(IEnumerable<ICommand> commands) =>
        _commands = commands;

    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var userId = message.From?.Id ?? throw new ArgumentNullException(nameof(message.From.Id));
        var (commandName, arguments) = ParseMessageText(message.Text!);
        var command = GetCommand(commandName, userId);
        var result = await command.ExecuteAsync(arguments, botClient);
        UpdateState(command, userId, result);

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: result.Text,
            replyMarkup: result.ReplyMarkup ?? new ReplyKeyboardRemove(),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    private (string, string[]) ParseMessageText(string text)
    {
        var arguments = text.Split(' ').Where(x => !string.IsNullOrEmpty(x));

        return (arguments.First(), arguments.ToArray());
    }

    private ICommand GetCommand(string name, long userId) =>
        GetCommandByName(name) ??
        GetCommandFromState(userId) ??
        GetDefaultCommand();

    private ICommand? GetCommandByName(string name) => 
        _commands.FirstOrDefault(x => x.Name == name);

    private ICommand? GetCommandFromState(long userId) =>
        _state.TryGetValue(userId, out var command) ? command : null;

    private ICommand GetDefaultCommand() =>
        _commands.First(x => x.Name == _defaultCommandName);

    private void UpdateState(ICommand command, long userId, CommandResult result)
    {
        if (result.ReplyMarkup is null)
        {
            _state.Remove(userId);
        }
        else
        {
            _state[userId] = command;
        }
    }
}
