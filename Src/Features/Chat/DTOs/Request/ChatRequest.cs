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
    public string UserId { get; set; } = string.Empty;
    public string? Message { get; set; } =string.Empty;

    public StatusSending statusSending { get; set; }

    //public List<IFormFile>? Media { get; set; } = [];

}


public class FileRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    public StatusSending statusSending { get; set; }

    public List<IFormFile>? Files { get; set; } = [];

}
