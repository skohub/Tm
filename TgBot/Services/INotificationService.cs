using Data.Models.Messasges;

namespace TgBot.Services;
public interface INotificationService
{
    Task SendAsync(IEnumerable<Message> messages);
}