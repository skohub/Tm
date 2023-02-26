using TgBot.Models;

namespace TgBot;
public class Configuration
{
    public string Token { get; set; } = string.Empty;
    public IList<long> AllowedUserIds { get; set; } = new List<long>();
    public int PollingIntervalMilliseconds { get; set; }
    public IList<NotificationSubscription> Subscriptions { get; set; } = new List<NotificationSubscription>();
}