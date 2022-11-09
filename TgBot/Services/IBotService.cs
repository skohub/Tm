using Telegram.Bot;
using Telegram.Bot.Types;

namespace Tm.TgBot.Services
{
    public interface IBotService
    {
        Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}