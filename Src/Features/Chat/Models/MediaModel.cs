namespace HUBTSOCIAL.Src.Features.Chat.Models;


public class MediaChatItem : ChatItem
{
    public List<string> MediaUrls { get; set; } = new List<string>();
    public override object ToResponseData() => new { MediaUrls };
}








