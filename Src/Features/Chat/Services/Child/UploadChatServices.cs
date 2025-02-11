using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public class UploadChatServices : IUploadChatServices
{
    private readonly IFileUploadService _fileUploadService;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IMediaUploadService _mediaUploadService;
    private readonly IMessageUploadService _messageUploadService;

    public UploadChatServices
    (
        IMessageUploadService messageUploadService,
        IMediaUploadService mediaUploadService,
        IFileUploadService fileUploadService,
        IHubContext<ChatHub> hubContext
    )
    {
        _messageUploadService = messageUploadService;
        _mediaUploadService = mediaUploadService;
        _fileUploadService = fileUploadService;
        _hubContext = hubContext;
    }


    public async Task<bool> SendChatAsync(ChatRequest chatRequest)
    {
        var tasks = new List<Task<bool>>();

        // Gửi tin nhắn nếu có
        if (chatRequest.Content != null)
            tasks.Add(Task.Run(async () =>
            {
                var messageRequest = new MessageRequest
                {
                    GroupId = chatRequest.GroupId,
                    Content = chatRequest.Content,
                    UserName = chatRequest.UserName
                };
                try
                {
                    return await _messageUploadService.UploadMessageAsync(messageRequest, _hubContext);
                }
                catch
                {
                    return false;
                }
            }));

        // Gửi media nếu có
        if (chatRequest.Medias != null)
            tasks.Add(Task.Run(async () =>
            {
                var mediaRequest = new MediaRequest
                {
                    GroupId = chatRequest.GroupId,
                    Medias = chatRequest.Medias,
                    UserName = chatRequest.UserName
                };
                try
                {
                    return await _mediaUploadService.UploadMediaAsync(mediaRequest, _hubContext);
                }
                catch
                {
                    return false;
                }
            }));

        // Gửi file nếu có
        // if (chatRequest.Medias != null)
        // {
        //     tasks.Add(Task.Run(async () =>
        //     {
        //         try { return await _fileUploadService.UploadFileAsync(file); }
        //         catch { return false; }
        //     }));
        // }

        // Chạy tất cả các request đồng thời và lấy kết quả
        var results = await Task.WhenAll(tasks);

        // Nếu ít nhất một request thành công, trả về true
        return results.Any(success => success);
    }
}