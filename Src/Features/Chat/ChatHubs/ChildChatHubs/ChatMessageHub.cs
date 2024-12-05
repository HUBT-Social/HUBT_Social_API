using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;

public class ChatMessageHub : Hub, IChatMessageHub
{
    private readonly IHubContext<ChatMessageHub> _hubContext;

    public ChatMessageHub(IHubContext<ChatMessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    ///     Gửi tin nhắn đến tất cả người dùng trong phòng chat.
    /// </summary>
    public async Task SendMessage(string GroupId, MessageModel messageModel)
    {
        try
        {
            // Gửi tin nhắn
            await _hubContext.Clients.Group(GroupId).SendAsync("ReceiveMessage", messageModel);
        }
        catch (Exception)
        {

        }
    }



}