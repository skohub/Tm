using System;

namespace Data.Models.Messasges;
public class Message
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Msg { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}