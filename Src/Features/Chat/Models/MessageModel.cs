using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models;
[BsonDiscriminator("MessageChatItem")]
public class MessageChatItem : ChatItem
{
    public string Content { get; set; } = string.Empty;
    public List<LinkMetadataModel> Links { get; set; } = new List<LinkMetadataModel>();
    public override object ToResponseData() => new { Content, Links };
}

