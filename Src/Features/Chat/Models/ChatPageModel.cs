namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class ChatPage
    {
        public string BlockId { get; set; }
        public List<MessageModel> Data { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
