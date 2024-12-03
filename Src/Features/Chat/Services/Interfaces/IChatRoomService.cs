using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IChatRoomService
{
    Task<string?> CreateGroupAsync(CreateGroupRequest createGroupRequest);
    Task<bool> UpdateGroupNameAsync(string idGroup, string newGroupName);
    Task<bool> DeleteGroupAsync(string idGroup);
    Task<ChatRoomModel> GetGroupByIdAsync(string id);
    Task<List<ChatRoomModel>> GetAllGroupsAsync();
}