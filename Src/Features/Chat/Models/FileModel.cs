using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models;
[BsonDiscriminator("FileChatItem")]
public class FileChatItem : ChatItem
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public override object ToResponseData() => new { FileUrl,FileName,FileSize,FileType };
}