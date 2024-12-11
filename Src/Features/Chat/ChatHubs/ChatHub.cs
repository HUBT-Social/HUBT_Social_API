using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs;

public class ChatHub : Hub, IChatHub
{
    private readonly IChatFileHub _chatFileHub;
    private readonly IChatMessageHub _chatMessageHub;

    public ChatHub(IChatMessageHub chatMessageHub, IChatFileHub chatFileHub)
    {
        _chatMessageHub = chatMessageHub;
        _chatFileHub = chatFileHub;
    }

    // Chuyển tiếp tin nhắn
    public async Task SendMessage(string GroupId, MessageChatItem messageModel)
    {
        await _chatMessageHub.SendMessage(GroupId,messageModel);
    }

    // Chuyển tiếp phương thức gửi file
    public async Task SendMedia(string chatRoomId,MediaChatItem mediaModels)
    {
        await _chatFileHub.SendMedia(chatRoomId,mediaModels);
    }
}