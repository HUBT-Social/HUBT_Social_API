using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;

namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageModel
{
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string sentBy { get; set; }
    public string? message { get; set; }
    public List<FilePaths>? filePaths { get; set; }
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public ReplyMessage? replyMessage { get; set; }
    public Reaction? reactions { get; set; } = new();
    public MessageType messageType { get; set; }
    public MessageStatus status { get; set; } = MessageStatus.Pending;
    //public MessageActionStatus actionStatus { get; set; } = MessageActionStatus.Normal;
    public TimeSpan? voiceMessageDuration { get; set; }

    // Constructor private để ép buộc dùng factory method
    private MessageModel(string sentBy,MessageType messageType, string? message = null, List<FilePaths>? filePaths = null,ReplyMessage? replyMessage = null)
    {
        this.sentBy = sentBy;
        this.message = message;
        this.filePaths = filePaths;
        this.messageType = messageType;
        this.filePaths = filePaths;
    }

    // Factory method cho tin nhắn văn bản
    public static async Task<MessageModel> CreateTextMessageAsync
    (
        string sentBy, 
        string content, 
        ReplyMessage? replyMessage = null
    )
    {
        var message = new MessageModel(sentBy,MessageType.Text, content,null,replyMessage);
        return message;
    }

    // Factory method cho tin nhắn có file
    public static async Task<MessageModel> CreateMediaMessageAsync
    (
        string sentBy, 
        List<FilePaths> filePaths, 
        ReplyMessage? replyMessage = null
    )
    {
        var message = new MessageModel(sentBy,MessageType.Media, null, filePaths,replyMessage);
        return message;
    }

   
}

