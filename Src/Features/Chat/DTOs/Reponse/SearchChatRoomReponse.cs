namespace HUBT_Social_API.Features.Chat.DTOs;

public class SearchChatRoomReponse
{
    public string Id { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int TotalNumber { get; set; }
}