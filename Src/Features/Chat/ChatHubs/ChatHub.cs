
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
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IUploadChatServices _uploadChatServices;
    protected readonly ITokenService _tokenService;
    private readonly IUserConnectionManager _userConnectionManager;
    

    public ChatHub
    (
        IHubContext<ChatHub> hubContext,
        IUploadChatServices uploadChatServices,
        ITokenService tokenService,
        IUserConnectionManager userConnectionManager      
    )
    {
        _hubContext = hubContext;
        _uploadChatServices = uploadChatServices;
        _tokenService = tokenService;
        _userConnectionManager = userConnectionManager;
    }

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
                _userConnectionManager.AddConnection(userName,connectionId);
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
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User.Identity?.Name;
        if (userName != null)
        {
            await Clients.Others.SendAsync("UserDisconnected", userName);
            _userConnectionManager.RemoveConnection(userName);
        }
        await base.OnDisconnectedAsync(exception);
    }
    

    // Gửi tin nhắn hoặc phương tiện đến nhóm
 
    // Chuyển tiếp tin nhắn
    public async Task SendMessage(MessageInputRequest messageInputRequest)
    {
        if (messageInputRequest != null)
    {
        Console.WriteLine($"GroupId: {messageInputRequest.GroupId}");
        Console.WriteLine($"Content: {messageInputRequest.Content}");
    }
    else
    {
        Console.WriteLine("Received null message.");
    }
        Console.WriteLine("GroupId: ", messageInputRequest.GroupId, "Content: ",messageInputRequest.Content);
        var userName = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
Console.WriteLine($"Đây là username lấy từ token: {userName}");

        if(userName == null)
        {
            await Clients.Caller.SendAsync("SendErr","Token no vali");
            return; 
        }
        MessageRequest messageRequest = new MessageRequest
        {
            UserName = userName,
            GroupId = messageInputRequest.GroupId,
            Content = messageInputRequest.Content
        };
        Console.WriteLine("Tạo thành công mesrqmesrq");
        await _uploadChatServices.SendMessageAsync(messageRequest,"ReceiveMessage");
        Console.WriteLine("Upload successsuccess");
    }

    // Chuyển tiếp phương tiện
    public async Task SendMedia(MediaInputRequest mediaInputRequest)
    {
        var userName = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if(userName == null)
        {
            await Clients.Caller.SendAsync("SendErr","Token no vali");
            return; 
        }
        MediaRequest mediaRequest = new MediaRequest
        {
            UserName = userName,
            GroupId = mediaInputRequest.GroupId,
            Files = mediaInputRequest.Files
        };
        await _uploadChatServices.SendMediaAsync(mediaRequest,"ReceiveMedia");
    }

    // Thông báo người dùng đang gõ
    public async Task TypingText(string groupId, string userId)
    {
        try
        {
            await _hubContext.Clients.Group(groupId).SendAsync("ReceiveTyping", userId);
        }
        catch (Exception ex)
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error notifying typing: {ex.Message}");
        }
    }



}

