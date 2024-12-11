namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ChatItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SenderId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = string.Empty; // Loại tin nhắn (Message, Media, File)
}






