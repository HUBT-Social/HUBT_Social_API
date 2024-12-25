using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models;
[BsonDiscriminator("VoiceChatItem")]
public class VoiceChatItem : ChatItem
{
    public string Url { get; set; } = string.Empty;
    public double Duration { get; set; } // Thời lượng âm thanh (giây)
    public string Format { get; set; } = string.Empty; // Định dạng (MP3, WAV, v.v.)
    public override object ToResponseData() => new { Url, Duration, Format};
}