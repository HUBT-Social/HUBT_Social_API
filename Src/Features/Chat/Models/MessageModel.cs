namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageModel
{
    public string Id {get; set;} = Guid.NewGuid().ToString(); 
    public string UserId { get; set; } = string.Empty;
    public List<string> Content { get; set; } = new();
    public Type Type { get; set; }
    public bool IsUnsend { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}
public enum Type
{
    Message,
    File
}