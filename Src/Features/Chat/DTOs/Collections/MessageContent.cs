using HUBTSOCIAL.Src.Features.Chat.Models;
namespace HUBTSOCIAL.Src.Features.Chat.Collections;
public class MessageContent
{
    public MessageContent(string Content)
    {
        this.Content = Content;
    }
    public string Content { get; set; } = string.Empty;
    public List<LinkMetadataModel> Links { get; set; } = new List<LinkMetadataModel>();
}