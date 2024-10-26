namespace HUBTSOCIAL.Src.Features.Chat.DTOs
{
    public class MessageDTO
    {
        public string UserId { get; set; } = String.Empty;
        public string ChatRoomId { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
    }
}
