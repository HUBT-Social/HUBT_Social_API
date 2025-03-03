namespace HUBT_Social_API.Src.Features.Notifcate.Models.Requests;

public class SendMessageRequest : MessageRequest
{
    public string Token { get; set; } = string.Empty;
}
public class SendGroupMessageRequest : MessageRequest
{
    public string GroupId { get; set; } = string.Empty;
}
public class MessageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public string? Type { get; set; } = "default";
}