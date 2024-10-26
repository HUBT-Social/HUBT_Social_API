using System;

namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class MessageModel
    {
        public string Id { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
