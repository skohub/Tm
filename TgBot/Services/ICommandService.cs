using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBot.Services
{
    public interface ICommandService
    {
        Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}