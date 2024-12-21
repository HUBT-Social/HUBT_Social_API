namespace HUBT_Social_API.Features.Chat.DTOs;
public class ChatHistoryResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; }
    public object? Data { get; set; }
}