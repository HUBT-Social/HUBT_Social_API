namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class AttachmentModel
    {
        public string Id { get; set; } = String.Empty;
        public string MessageId { get; set; } = String.Empty;
        public string FileUrl { get; set; } = String.Empty;
        public string FileType { get; set; } = String.Empty;// Image, Video, etc.
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; } 
    }
}
