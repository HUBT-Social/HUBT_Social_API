using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.Services.Child
{
    public static class SendingItem
    {
        // Gửi tin nhắn hoặc phương tiện đến nhóm
        public static async Task SendChatItem<T>(string groupId, T chatItem, string eventName,IHubContext<ChatHub> _hubContext) where T : ChatItem
        {
            try
            {
                var chatItemResponse = new ChatItemResponse
                {
                    Id = chatItem.Id,
                    NickName = await RoomChatHelper.GetNickNameAsync(groupId, chatItem.UserName) ?? chatItem.UserName,
                    UserName = chatItem.UserName,
                    Timestamp = chatItem.Timestamp,
                    Type = chatItem.Type,
                    Data = chatItem.ToResponseData()
                };
                    Console.WriteLine(chatItemResponse);
                await _hubContext.Clients.Group(groupId).SendAsync(eventName, chatItemResponse);
                Console.WriteLine("chatItemResponse");
            }
            catch (Exception ex)
            {
                // Log lỗi và xử lý tùy theo nhu cầu
                Console.WriteLine($"Error sending {eventName}: {ex.Message}");
                Console.WriteLine("LoiLoi");
            }
        }
    }
}