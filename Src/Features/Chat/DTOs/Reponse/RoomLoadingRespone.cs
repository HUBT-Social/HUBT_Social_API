namespace HUBT_Social_API.Features.Chat.DTOs;

public class RoomLoadingRespone : RoomBaseReponse
{
    public string LastMessage { get; set; } = string.Empty;
    public string LastInteractionTime { get; set; } = string.Empty;
}