
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs;

public class ChatHub : Hub
{

    private readonly IHubContext<ChatHub> _hubContext;

    public ChatHub(IHubContext<ChatHub> hubContext)
    {

        _hubContext = hubContext;
    }

    // Chuyển tiếp tin nhắn
    public async Task SendMessage(string GroupId, MessageChatItem messageModel)
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
    // Chuyển tiếp phương thức gửi file
    public async Task SendMedia(string chatRoomId,MediaChatItem mediaModels)
    {
        await _hubContext.Clients.Group(chatRoomId).SendAsync("SendMedia", mediaModels);
    }


    public async Task TypingText(string chatRoomId, string userId)
    {
        await _hubContext.Clients.Group(chatRoomId).SendAsync("typing", userId);
    }

}