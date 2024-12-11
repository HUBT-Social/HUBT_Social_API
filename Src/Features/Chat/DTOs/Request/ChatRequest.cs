namespace HUBT_Social_API.Features.Chat.DTOs;

public enum StatusSending
{
    Sending,
    Sent,
    SendingFaild
}
public class MessageRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string? Content { get; set; } =string.Empty;
}


public class FileRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public List<IFormFile>? Files { get; set; } = [];

}
