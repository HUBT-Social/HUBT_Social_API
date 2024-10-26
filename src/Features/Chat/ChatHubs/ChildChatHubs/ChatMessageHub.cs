using Microsoft.AspNetCore.SignalR;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Services.IChatServices;
using System.Threading.Tasks;
using HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs;

namespace HUBTSOCIAL.Src.Features.Chat.Hubs.ChildChatHubs
{
    public class ChatMessageHub : Hub, IChatMessageHub
    {
        private readonly IChatService _chatService;

        public ChatMessageHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Gửi tin nhắn đến tất cả người dùng trong phòng chat.
        /// </summary>
        public async Task SendMessage(string chatRoomId, string userId, string messageContent)
        {
            var messageDto = new MessageDTO
            {
                UserId = userId,
                ChatRoomId = chatRoomId,
                Content = messageContent
            };

            // Gửi tin nhắn đến tất cả các người dùng trong phòng chat
            await Clients.Group(chatRoomId).SendAsync("ReceiveMessage", new { UserId = userId, MessageContent = messageDto });

            // Lưu tin nhắn vào MongoDB
            await _chatService.SendMessageAsync(messageDto);
        }

        /// <summary>
        /// Người dùng tham gia vào phòng chat.
        /// </summary>
        public async Task JoinRoom(string chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
            await Clients.Group(chatRoomId).SendAsync("UserJoined", Context.ConnectionId);
        }

        /// <summary>
        /// Người dùng rời khỏi phòng chat.
        /// </summary>
        public async Task LeaveRoom(string chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
            await Clients.Group(chatRoomId).SendAsync("UserLeft", Context.ConnectionId);
        }
    }
}
