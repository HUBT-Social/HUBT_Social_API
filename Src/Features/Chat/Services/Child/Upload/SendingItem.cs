using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public static class SendingItem
{
    // Gửi tin nhắn hoặc phương tiện đến nhóm
    public static async Task SendChatItem(string groupId, MessageModel chatItem, IHubContext<ChatHub> _hubContext)
    {
        try
        {
            await _hubContext.Clients.Group(groupId).SendAsync("ReceiveChat", chatItem);
            Console.WriteLine("chatItemResponse");
        }
        catch (Exception ex)
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error sending ReceiveChat: {ex.Message}");
            Console.WriteLine("LoiLoi");
        }
    }
}