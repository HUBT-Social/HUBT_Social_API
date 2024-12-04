namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class MessageModel : BaseChatModel
{
    public string Content { get; set; } = string.Empty;
    public List<LinkMetadataModel> Links { get; set; } = new();

}

