namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageModel
{
    public string Id {get; set;} = Guid.NewGuid().ToString(); 
    public string UserId { get; set; } = string.Empty;
    public List<string> Content { get; set; } = new();
    public MessageType Type { get; set; }
    public bool IsUnsend { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}
public enum MessageType
{
    Message,
    MessageLink,
    Media,
    File
}

public class LinkMetadata
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}