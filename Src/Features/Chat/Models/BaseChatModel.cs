namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class BaseChatModel
{
    public string Id {get; set;} = Guid.NewGuid().ToString(); 
    public string SenderId { get; set; } = string.Empty;
    public bool IsUnsend { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}