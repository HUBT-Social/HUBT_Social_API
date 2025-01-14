namespace HUBT_Social_API.Features.Chat.DTOs;
public class ChatItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ReplyTo { get; set; } = string.Empty;
    public bool Unsend { get; set; } = false;
    public bool IsPin { get; set; } = false;
    public object? Data { get; set; }
}