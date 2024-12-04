namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class FileModel : BaseChatModel
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty; // Tên tệp
    public long FileSize { get; set; } // Kích thước tệp (bytes)
    public string FileType { get; set; } = string.Empty; // Loại tệp (PDF, DOCX, v.v.)
}