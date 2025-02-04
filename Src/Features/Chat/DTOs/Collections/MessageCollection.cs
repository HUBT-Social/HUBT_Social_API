using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageCollection
{
    /// <summary>
    /// Provides Id
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Provides actual message content as text
    /// </summary>
    public MessageContent? MessageContent { get; set; }

    /// <summary>
    /// Provides file paths for images or audio files
    /// </summary>
    public List<FilePaths>? FilePaths { get; set; }

    /// <summary>
    /// Provides message created date and time
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Provides UserName of sender of the message
    /// </summary>

    /// <summary>
    /// Provides reply message if the user replies to any message
    /// </summary>
    public ReplyMessage? ReplyMessage { get; set; }

    /// <summary>
    /// Represents reaction on the message
    /// </summary>
    public List<Reaction> Reactions { get; set; } = new();

    /// <summary>
    /// Provides message type
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// Status of the message
    /// </summary>
    public MessageStatus Status { get; set; } = MessageStatus.Pending;
    public MessageActionStatus ActionStatus { get; set; } = MessageActionStatus.Normal;

    /// <summary>
    /// Provides max duration for recorded voice message
    /// </summary>
    public TimeSpan? VoiceMessageDuration { get; set; }

    public MessageCollection(MessageCollection messageCollection)
    {
        this.Id = messageCollection.Id;
        this.MessageContent = messageCollection.MessageContent;
        this.FilePaths = messageCollection.FilePaths;
        this.ReplyMessage = messageCollection.ReplyMessage;
        this.VoiceMessageDuration = messageCollection.VoiceMessageDuration;

    }
    public MessageCollection(MessageContent MessageContent,string? roomId = null,string? messageId = null)
    {

        this.MessageContent = MessageContent;
        if(messageId != null || roomId != null)
        {
            Task<MessageModel?> message = RoomChatHelper.GetInfoMessageAsync(roomId, messageId);
            if(message != null)
            {
                string replyTo = RoomChatHelper.GetNickNameAsync(roomId,message.Result.SentBy).ToString();
                ReplyMessage replyMessage = new ReplyMessage
                {
                    messageId = message.Result.Id,
                    ReplyTo = replyTo,
                };
                if(message.Result.MessageType == MessageType.Text)
                {
                    replyMessage.Message = message.Result.MessageContent.Content;
                }
                else if((message.Result.MessageType & MessageType.Media) != 0)
                {
                    replyMessage.FirstMediaUrl = message.Result.FilePaths.FirstOrDefault().Url;
                }
                else if(message.Result.MessageType == MessageType.Voice)
                {
                    replyMessage.voiceMessageDuration = message.Result.VoiceMessageDuration;
                }
                else
                {
                    
                }

                this.ReplyMessage = replyMessage;     
            }

            
        }
        

    }
    public MessageCollection(List<FilePaths> FilePaths,string? roomId = null,string? messageId = null)
    {

        this.FilePaths = FilePaths;
        if(messageId != null || roomId != null)
        {
            Task<MessageModel?> message = RoomChatHelper.GetInfoMessageAsync(roomId, messageId);
            if(message != null)
            {
                string replyTo = RoomChatHelper.GetNickNameAsync(roomId,message.Result.SentBy).ToString();
                ReplyMessage replyMessage = new ReplyMessage
                {
                    messageId = message.Result.Id,
                    ReplyTo = replyTo,
                };
                if(message.Result.MessageType == MessageType.Text)
                {
                    replyMessage.Message = message.Result.MessageContent.Content;
                }
                else if((message.Result.MessageType & MessageType.Media) != 0)
                {
                    replyMessage.FirstMediaUrl = message.Result.FilePaths.FirstOrDefault().Url;
                }
                else if(message.Result.MessageType == MessageType.Voice)
                {
                    replyMessage.voiceMessageDuration = message.Result.VoiceMessageDuration;
                }
                else
                {
                    
                }

                this.ReplyMessage = replyMessage;     
            }

            
        }
        

    }
}
