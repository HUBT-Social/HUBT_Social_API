namespace HUBT_Social_API.Features.Chat.DTOs;
public class ChatItemResponse
{
    public string Id { get; set; }
    public string NickName { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; }
    public object? Data { get; set; }
}