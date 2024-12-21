
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs;

public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserService _userService;
    private readonly IRoomService _roomService;

    public ChatHub(IHubContext<ChatHub> hubContext, IUserService userService, IRoomService roomService)
    {
        _hubContext = hubContext;
        _userService = userService;
        _roomService = roomService;
    }

    // Gửi tin nhắn hoặc phương tiện đến nhóm
    private async Task SendChatItem<T>(string groupId, T chatItem, string eventName) where T : ChatItem
    {
        try
        {
            var chatItemResponse = new ChatItemResponse
            {
                Id = chatItem.Id,
                NickName = await _roomService.GetNickNameAsync(groupId, chatItem.UserName) ?? chatItem.UserName,
                AvatarUrl = await _userService.GetAvatarUrlFromUserName(chatItem.UserName),
                Timestamp = chatItem.Timestamp,
                Type = chatItem.Type,
                Data = chatItem.ToResponseData()
            };

            await _hubContext.Clients.Group(groupId).SendAsync(eventName, chatItemResponse);
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
    public async Task TypingText(string roomId, string userId)
    {
        try
        {
            await _hubContext.Clients.Group(roomId).SendAsync("ReceiveSignalTyping", userId);
        }
        catch (Exception ex)
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error notifying typing: {ex.Message}");
        }
    }
}
