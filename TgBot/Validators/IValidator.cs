using Telegram.Bot.Types;

namespace TgBot.Validators;
public interface IValidator
{
    Task<bool> ValidateAsync(Message message);
}