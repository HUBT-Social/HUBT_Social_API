
namespace HUBTSOCIAL.Src.Features.Chat.Models;
public class LinkMetadataModel
{
    public string Url { get; set; } = string.Empty; // Link URL
    public string Title { get; set; } = "No Title"; // Tiêu đề của trang web
    public string Description { get; set; } = string.Empty; // Mô tả (nếu có)
    public string Thumbnail { get; set; } = string.Empty; // URL của ảnh thumbnail

    public static implicit operator LinkMetadataModel(List<ChatRoomModel> v)
    {
        throw new NotImplementedException();
    }
}