using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using static HUBT_Social_API.Features.Chat.Services.ChatService;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IChatService
{
    Task<string?> CreateGroupAsync(ChatRoomModel newRoomModel);
    Task<bool> DeleteGroupAsync(string idGroup);
    Task<ChatRoomModel> GetGroupByIdAsync(string id);
    Task<List<SearchChatRoomReponse>> SearchGroupsAsync(string keyword);
    Task<List<ChatRoomModel>> GetAllRoomsAsync();
    Task<List<ChatRoomModel>> GetRoomsByUserNameAsync(string userName);

}