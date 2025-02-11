using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SentBy { get; set; }
    public MessageContent? MessageContent { get; set; }
    public List<FilePaths>? FilePaths { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ReplyMessage? ReplyMessage { get; set; }
    public List<Reaction> Reactions { get; set; } = new();
    public MessageType MessageType { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.Pending;
    public MessageActionStatus ActionStatus { get; set; } = MessageActionStatus.Normal;
    public TimeSpan? VoiceMessageDuration { get; set; }

    // Constructor private để ép buộc dùng factory method
    private MessageModel(string sentBy,MessageType messageType, MessageContent? content = null, List<FilePaths>? filePaths = null)
    {
        SentBy = sentBy;
        MessageContent = content;
        FilePaths = filePaths;
        MessageType = messageType;
    }

    // Factory method cho tin nhắn văn bản
    public static async Task<MessageModel> CreateTextMessageAsync(string sentBy, MessageContent content, string? roomId = null, string? messageId = null)
    {
        var message = new MessageModel(sentBy,MessageType.Text, content,null);
        if (!string.IsNullOrEmpty(roomId) && !string.IsNullOrEmpty(messageId))
        {
            message.ReplyMessage = await GetReplyMessageAsync(roomId, messageId);
        }
        return message;
    }

    // Factory method cho tin nhắn có file
    public static async Task<MessageModel> CreateMediaMessageAsync(string sentBy, List<FilePaths> filePaths, string? roomId = null, string? messageId = null)
    {
        var message = new MessageModel(sentBy,MessageType.Media, null, filePaths);
        if (!string.IsNullOrEmpty(roomId) && !string.IsNullOrEmpty(messageId))
        {
            message.ReplyMessage = await GetReplyMessageAsync(roomId, messageId);
        }
        return message;
    }

    // Hàm lấy thông tin trả lời tin nhắn
    private static async Task<ReplyMessage?> GetReplyMessageAsync(string? roomId, string? messageId)
    {
        if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(messageId)) return null;

        var originalMessage = await RoomChatHelper.GetInfoMessageAsync(roomId, messageId);
        if (originalMessage == null) return null;

        var replyTo = await RoomChatHelper.GetNickNameAsync(roomId, originalMessage.SentBy);
        var replyMessage = new ReplyMessage
        {
            messageId = originalMessage.Id,
            ReplyTo = replyTo
        };

        if (originalMessage.MessageType == MessageType.Text)
        {
            replyMessage.Message = originalMessage.MessageContent?.Content;
        }
        else if ((originalMessage.MessageType & MessageType.Media) != 0)
        {
            replyMessage.FirstMediaUrl = originalMessage.FilePaths?.FirstOrDefault()?.Url;
        }
        else if (originalMessage.MessageType == MessageType.Voice)
        {
            replyMessage.voiceMessageDuration = originalMessage.VoiceMessageDuration;
        }

        return replyMessage;
    }
}

