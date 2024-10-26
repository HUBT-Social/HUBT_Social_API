using System.Collections.Generic;

namespace HUBTSOCIAL.Src.Features.Chat.DTOs
{
    public class ChatRoomDTO
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public List<string>? UserIds { get; set; } =  new List<string>();
        public List<MessageDTO>? Messages { get; set; } = new List<MessageDTO> {};
    }
}
