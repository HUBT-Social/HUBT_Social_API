using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using static HUBT_Social_API.Features.Chat.Services.RoomService;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IRoomService
{
    //Update methods
    Task<bool> UpdateGroupNameAsync(string idGroup, string newGroupName);

    //Get methods
    Task<IEnumerable<ChatHistoryResponse>> GetChatHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest);
    Task<IEnumerable<ItemsHistoryRespone>> GetItemsHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest);
}