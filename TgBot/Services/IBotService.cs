using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBot.Services
{
    public interface IBotService
    {
        Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}