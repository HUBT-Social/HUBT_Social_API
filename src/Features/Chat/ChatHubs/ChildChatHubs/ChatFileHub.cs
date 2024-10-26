using Microsoft.AspNetCore.SignalR;
using HUBTSOCIAL.Src.Features.Chat.Services.IChatServices;
using System.Threading.Tasks;
using HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs;

namespace HUBTSOCIAL.Src.Features.Chat.Hubs.ChildChatHubs
{
    public class ChatFileHub : Hub, IChatFileHub
    {
        private readonly IChatService _chatService;

        public ChatFileHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Gửi tệp đến tất cả người dùng trong phòng chat.
        /// </summary>
        public async Task SendFile(string chatRoomId, string userId, byte[] fileData, string fileName)
        {
            // Giả sử bạn đã có phương thức UploadFileAsync trong IChatService
            var fileUrl = await _chatService.UploadFileAsync(chatRoomId, fileData, fileName);
            if (fileUrl)
            {
                await Clients.Group(chatRoomId).SendAsync("ReceiveFile", new { UserId = userId, FileName = fileName, FileUrl = fileUrl });
            }
        }
    }
}
