namespace HUBT_Social_API.Features.Chat.DTOs;
public class ChatItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public object? Data { get; set; }
}