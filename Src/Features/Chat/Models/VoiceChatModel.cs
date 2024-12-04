namespace HUBTSOCIAL.Src.Features.Chat.Models;
public class VoiceChatModel : BaseChatModel
{
    public string Url { get; set; } = string.Empty;
    public double Duration { get; set; } // Thời lượng âm thanh (giây)
    public string Format { get; set; } = string.Empty; // Định dạng (MP3, WAV, v.v.)
}