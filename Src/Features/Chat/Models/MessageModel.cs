namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageModel
{
    public string UserId { get; set; } = string.Empty;
    public ContentModel Content { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}