
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBTSOCIAL.Src.Features.Chat.Services.IChatServices
{
    public interface IMessageService
    {
        Task<bool> DeleteMessageAsync(string chatRoomId, string messageId);
        Task<List<MessageModel>?> GetMessagesInChatRoomAsync(string chatRoomId);
        Task<List<MessageModel>> SearchMessagesInChatRoomAsync(string chatRoomId, string keyword);
        Task<bool> SendMessageAsync(string chatRoomId, string userId, string content);
    }
}