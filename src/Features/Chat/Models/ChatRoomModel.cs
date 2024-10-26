using System.Collections.Generic;

namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class ChatRoomModel
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public List<string>? UserIds { get; set; } = new List<string>();
        public List<MessageModel>? Messages { get; set; } = new List<MessageModel>();
        public DateTime CreatedAt { get; set; }
    }
}
