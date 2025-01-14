using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models;

[BsonDiscriminator(Required = true)]
[BsonKnownTypes(typeof(MessageChatItem))]
[BsonKnownTypes(typeof(MediaChatItem))]
[BsonKnownTypes(typeof(FileChatItem))]
[BsonKnownTypes(typeof(VoiceChatItem))]

public class ChatItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = string.Empty; // Loại tin nhắn (Message, Media, File)
    public List<string> MemberSeens { get; set; } = new();
    public bool Unsend { get; set; } = false;
    public bool IsPin { get; set; } = false;
    public string ReplyTo { get; set; } = string.Empty;
    public virtual object ToResponseData() => null;
}






