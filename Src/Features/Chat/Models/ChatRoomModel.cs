using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ChatRoomModel
{
    [BsonId]
    public string Id { get; set; } = string.Empty; // Id của phòng chat
    public string Name { get; set; } = string.Empty; // Tên của phòng chat
    public string AvatarUrl { get; set; } = string.Empty; // URL ảnh đại diện của phòng chat
    public string BackGroundUrl { get; set; } = string.Empty;
    public List<Participant> Participant { get; set; } = new(); // Danh sách ID người dùng tham gia phòng chat
    public List<ChatItem> ChatItems { get; set; } = new(); // Danh sách các tin nhắn, file, media trong phòng chat
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo phòng chat
    public TypeChatRoom TypeChatRoom { get; set; } = TypeChatRoom.None;

}
public class Participant
{
    /// <summary>
    /// UserName dùng để định danh người dùng.
    /// </summary>
    public string UserName { get; set; } = string.Empty; // Id của người dùng
    public ParticipantRole Role { get; set; } = ParticipantRole.Member; // Vai trò của người dùng (vd: Admin, Member)
    public string NickName { get; set; } = string.Empty;
    public DateTime LastInteractionTime { get; set; } // Thời gian tương tác gần nhất
}
public enum TypeChatRoom
{
    /// <summary>
    /// 
    /// </summary>
    SingleChat, 
    GroupChat,
    None
}
public enum ParticipantRole
{
    Member,
    Admin,
    Owner
    
}
