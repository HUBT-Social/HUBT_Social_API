namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageChatItem : ChatItem
{
    public string Content { get; set; } = string.Empty;
    public List<LinkMetadataModel> Links { get; set; } = new List<LinkMetadataModel>();
    public override object ToResponseData() => new { Content, Links };
}

