
using System.Security.Claims;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs;

public class ChatHub : Hub
{

    public override async Task OnConnectedAsync()
    {
        try
        {
            var connectionId = Context.ConnectionId;
            
            // Lấy UserName từ Claims
            var userName = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (userName != null)
            {
                // Sử dụng userName thay vì userId
                var groupIds = await RoomChatHelper.GetUserGroupConnected(userName);  // Giả sử bạn lưu nhóm theo userName

                foreach (var groupId in groupIds)
                {
                    await Groups.AddToGroupAsync(connectionId, groupId);
                    await Clients.Group(groupId).SendAsync("UserRejoined", connectionId);
                }
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reconnecting user: {ex.Message}");
        }
    }
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disconnecting user: {ex.Message}");
        }
    }
    

    // Gửi tin nhắn hoặc phương tiện đến nhóm
    private async Task SendChatItem<T>(string groupId, T chatItem, string eventName) where T : ChatItem
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

            await Clients.Group(groupId).SendAsync(eventName, chatItemResponse);
        }
        catch (Exception ex)
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error sending {eventName}: {ex.Message}");
        }
    }

    // Chuyển tiếp tin nhắn
    public async Task SendMessage(string groupId, MessageChatItem messageModel)
    {
        await SendChatItem(groupId, messageModel, "ReceiveMessage");
    }

    // Chuyển tiếp phương tiện
    public async Task SendMedia(string groupId, MediaChatItem mediaModel)
    {
        await SendChatItem(groupId, mediaModel, "ReceiveMedia");
    }

    // Thông báo người dùng đang gõ
    public async Task TypingText(string groupId, string userId)
    {
        try
        {
            await Clients.Group(groupId).SendAsync("ReceiveTyping", userId);
        }
        catch (Exception ex)
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error notifying typing: {ex.Message}");
        }
    }
    //Thông báo có người gỡ tin nhắn.
    public async Task UnSendChatItem(string groupId, string MessageId)
    {
        try
        {
            await Clients.Group(groupId).SendAsync("ReceiveUnSendItem", MessageId);
        }
        catch
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error unsend message:");
        }
    }
    // Phương thức JoinRoom
    public async Task JoinRoom(string groupId, string userId)
    {
        try
        {
            // Kiểm tra nếu người dùng đã ở trong nhóm
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("JoinGroup", userId);
            Console.WriteLine($"User {userId} joined group {groupId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error joining group {groupId}: {ex.Message}");
        }
    }

    // Phương thức LeaveRoom
    public async Task LeaveRoom(string groupId, string userId)
    {
        try
        {
            await Clients.Group(groupId).SendAsync("LeaveRoom", userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            Console.WriteLine($"User {userId} left group {groupId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error leaving group {groupId}: {ex.Message}");
        }
    }
}

