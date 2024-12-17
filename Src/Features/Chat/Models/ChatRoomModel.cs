using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ChatRoomModel
{
    [BsonId]
    public string ChatRoomId { get; set; } = Guid.NewGuid().ToString(); // Id của phòng chat
    [BsonIgnore]
    public string ObjectId { get; set; } // Bỏ qua trường này khi lưu vào MongoDB
    public string Name { get; set; } = string.Empty; // Tên của phòng chat
    public string AvatarUrl { get; set; } = string.Empty; // URL ảnh đại diện của phòng chat
    public List<string> UserIds { get; set; } = new(); // Danh sách ID người dùng tham gia phòng chat
    public List<ChatItem> ChatItems { get; set; } = new(); // Danh sách các tin nhắn, file, media trong phòng chat
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo phòng chat
}
