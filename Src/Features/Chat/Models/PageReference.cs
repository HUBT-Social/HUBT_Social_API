namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class PageReference
    {
        public string BlockId { get; set; } = Guid.NewGuid().ToString();
        public string PreBlockId { get; set; } = string.Empty; // Khởi tạo rỗng
    }
}
