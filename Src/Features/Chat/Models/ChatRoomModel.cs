namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ChatRoomModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public List<string>? UserIds { get; set; } = new(); // Danh sách ID người dùng
    public List<MessageModel>? Messages { get; set; } = new(); // Tin nhắn
    public List<MediaModel>? MediaItems { get; set; } = new(); 
    public List<FileModel>? FileItems { get; set; } = new();
    public List<LinkMetadataModel>? Links { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
}
