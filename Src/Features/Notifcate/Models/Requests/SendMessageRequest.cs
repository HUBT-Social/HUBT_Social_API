namespace HUBT_Social_API.Src.Features.Notifcate.Models.Requests;

public class SendMessageRequest
{
    public string Token { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}