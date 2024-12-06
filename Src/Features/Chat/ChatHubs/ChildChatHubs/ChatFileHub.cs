using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;

public class ChatFileHub : Hub, IChatFileHub
{
    private readonly IHubContext<ChatMessageHub> _hubContext;

    public ChatFileHub(IHubContext<ChatMessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    ///     Gửi tệp đến tất cả người dùng trong phòng chat.
    /// </summary>
    public async Task SendMedia(string chatRoomId, MediaChatItem mediaModels)
    {
        await _hubContext.Clients.Group(chatRoomId).SendAsync("SendMedia", mediaModels);
    }
}