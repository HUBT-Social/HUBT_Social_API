namespace HUBT_Social_API.Features.Chat.DTOs;

public class ChatRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? Message { get; set; } =string.Empty;
    public List<IFormFile>? Media { get; set; } = [];
}