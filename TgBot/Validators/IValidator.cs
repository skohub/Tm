using Telegram.Bot.Types;

namespace Tm.TgBot.Validators;
public interface IValidator
{
    Task<bool> ValidateAsync(Message message);
}