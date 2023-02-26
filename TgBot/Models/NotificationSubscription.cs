namespace TgBot.Models;
public class NotificationSubscription
{
    public string Type { get; set; } = string.Empty;
    public string UserPattern { get; set; } = string.Empty;
    // Template can contain @msg, @date, @user tags
    public string NotificationTemplate { get; set; } = string.Empty;
    public int ChatId { get; set; }
}