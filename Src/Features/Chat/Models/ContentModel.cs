namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ContentModel
{
    public string Id { get; set; } = string.Empty;
    public string? Message { get; set; } = string.Empty;
    public List<Media> Media { get; set; } = new(); // Danh sách các tệp (ảnh, video, file, audio)
 
}

public class Media
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Id duy nhất
    public string Url { get; set; } = string.Empty;            // URL file
    public string FileType { get; set; } = string.Empty;       // Loại file (MIME type)
    public long Size { get; set; }                             // Kích thước file
    public MediaType MediaType { get; set; }                   // Loại nội dung (ảnh, video, file, audio)
    public bool IsUnsend { get; set; } = false; // Trạng thái đã gỡ
}

// Định nghĩa loại Media
public enum MediaType
{
    Image,
    File,
    Video,
    Audio
}