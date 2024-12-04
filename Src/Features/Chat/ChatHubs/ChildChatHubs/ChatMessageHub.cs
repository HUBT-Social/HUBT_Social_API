using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;

public class ChatMessageHub : Hub, IChatMessageHub
{


    public ChatMessageHub()
    {
    }

    /// <summary>
    ///     Gửi tin nhắn đến tất cả người dùng trong phòng chat.
    /// </summary>
    public async Task SendMessage(string GroupId, MessageModel messageModel)
    {

            // Send the message to all clients in the specified group
            await Clients.Group(GroupId).SendAsync("ReceiveMessage", new { messageModel.SenderId, Content = messageModel.Content }); 

        
    }


}