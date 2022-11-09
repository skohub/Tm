using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tm.TgBot.Commands.Common;

namespace Tm.TgBot.Services;
public class BotService : IBotService
{
    private IEnumerable<ICommand> _commands;

    public BotService(IEnumerable<ICommand> commands)
    {
        _commands = commands;
    }

    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var commandArguments = message.Text!.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        var command = GetCommand(commandArguments.First());
        var result = await command.ExecuteAsync(commandArguments, botClient);
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: result.Text,
            replyMarkup: result.ReplyMarkup ?? new ReplyKeyboardRemove(),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    private ICommand GetCommand(string name) =>
        _commands.FirstOrDefault(x => x.Name == name) ?? _commands.First(x => x.Name == "/help");
}
