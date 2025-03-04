using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class ChatHistory
    {
        [BsonId]
        public string RoomId { get; set; }
        public List<ChatPage> HistoryChat { get; set; } = new();
    }
}
