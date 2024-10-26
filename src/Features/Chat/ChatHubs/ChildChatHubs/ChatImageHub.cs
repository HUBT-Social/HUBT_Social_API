using Microsoft.AspNetCore.SignalR;
using HUBTSOCIAL.Src.Features.Chat.Services.IChatServices;
using System.Threading.Tasks;
using HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs;

namespace HUBTSOCIAL.Src.Features.Chat.Hubs.ChildChatHubs
{
    public class ChatImageHub : Hub, IChatImageHub
    {
        private readonly IChatService _chatService;

        public ChatImageHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Gửi hình ảnh đến tất cả người dùng trong phòng chat.
        /// </summary>
        public async Task SendImage(string chatRoomId, string userId, byte[] imageData)
        {
            // Giả sử bạn đã có phương thức UploadImageAsync trong IChatService
            var imageUrl = await _chatService.UploadImageAsync(userId, chatRoomId, imageData);
            if (imageUrl)
            {
                await Clients.Group(chatRoomId).SendAsync("ReceiveImage", new { UserId = userId, ImageUrl = imageUrl });
            }
        }
    }
}
