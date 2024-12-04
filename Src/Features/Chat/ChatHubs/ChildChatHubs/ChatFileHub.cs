using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;

public class ChatFileHub : Hub, IChatFileHub
{
    private readonly IChatService _chatService;

    public ChatFileHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    ///     Gửi tệp đến tất cả người dùng trong phòng chat.
    /// </summary>
    public async Task SendMedia(string chatRoomId, List<MediaModel> mediaModels)
    {
        string sender = mediaModels[0].SenderId;

        List<string> urls = mediaModels.Select(m => m.Url).ToList();

        await Clients.Group(chatRoomId).SendAsync("SendMedia", new { senderId = sender, urls = urls });
    }
}