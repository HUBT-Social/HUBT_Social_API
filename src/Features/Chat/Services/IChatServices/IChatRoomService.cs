using System.Threading.Tasks;
using System.Collections.Generic;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBTSOCIAL.Src.Features.Chat.Services.IChatServices
{
    public interface IChatRoomService
    {
        Task<bool> CreateGroupAsync(string nameGroup);
        Task<bool> UpdateGroupNameAsync(string idGroup, string newGroupName);
        Task<bool> DeleteGroupAsync(string idGroup);
        Task<ChatRoomModel> GetGroupByIdAsync(string id);
        Task<List<ChatRoomModel>> GetAllGroupsAsync();
    }
}