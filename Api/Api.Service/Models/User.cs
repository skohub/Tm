namespace Api.Service.Models
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? ConnectionStringName { get; set; }
    }
}