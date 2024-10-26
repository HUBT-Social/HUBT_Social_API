using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs;

namespace HUBTSOCIAL.Src.Features.Chat.ChatHubs
{
    public class ChatHub : Hub
    {
        private readonly IChatMessageHub _chatMessageHub;
        private readonly IChatImageHub _chatImageHub;
        private readonly IChatFileHub _chatFileHub;

        public ChatHub(IChatMessageHub chatMessageHub, IChatFileHub chatFileHub, IChatImageHub chatImageHub)
        {
            _chatMessageHub = chatMessageHub;
            _chatImageHub = chatImageHub;
            _chatFileHub = chatFileHub;
        }

        // Chuyển tiếp tin nhắn
        public async Task SendMessage(string chatRoomId, string userId, string messageContent)
        {
            await _chatMessageHub.SendMessage(chatRoomId, userId, messageContent);
        }

        // Tham gia vào phòng chat
        public async Task JoinRoom(string chatRoomId)
        {
            await _chatMessageHub.JoinRoom(chatRoomId);
        }

        // Rời khỏi phòng chat
        public async Task LeaveRoom(string chatRoomId)
        {
            await _chatMessageHub.LeaveRoom(chatRoomId);
        }
        //Chuyển tiếp phương thức gửi anh
        public async Task SendImage(string chatRoomId, string userId, byte[] fileData)
        {
            await _chatImageHub.SendImage(chatRoomId,userId,fileData);
        }

        // Chuyển tiếp phương thức gửi file
        public async Task SendFile(string chatRoomId, string userId, byte[] fileData, string fileName)
        {
            await _chatFileHub.SendFile(chatRoomId, userId, fileData, fileName);
        }
    }
}
