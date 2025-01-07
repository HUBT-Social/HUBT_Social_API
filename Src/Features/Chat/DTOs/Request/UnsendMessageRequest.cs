namespace HUBT_Social_API.Features.Chat.DTOs;
public class UnsendMessageRequest
{
    public string UserId { get; set; } = string.Empty;    // Id của người dùng thực hiện gỡ tin nhắn
    public string ChatRoomId { get; set; } = string.Empty; // Id của nhóm chat (hoặc cuộc trò chuyện)
    public string MessageId { get; set; } = string.Empty; // Id của tin nhắn cần gỡ
    
}
