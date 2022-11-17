namespace TgBot;
public class Configuration
{
    public string Token { get; set; } = string.Empty;
    public IList<long> AllowedUserIds { get; set; } = new List<long>();
}