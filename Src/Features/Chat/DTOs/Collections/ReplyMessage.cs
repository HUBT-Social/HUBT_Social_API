
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;

public class ReplyMessage
{
    public string? Message { get; set;} = string.Empty;
    public string? FirstMediaUrl { get; set;} = string.Empty;
    //name of message's owner
    public string ReplyTo { get; set;} = string.Empty;
    public MessageType messageType { get; set;} = MessageType.None;
    public TimeSpan? voiceMessageDuration { get; set;} = TimeSpan.Zero;
    public string messageId { get; set;} = string.Empty;
}