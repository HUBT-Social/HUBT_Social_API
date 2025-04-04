using HUBT_Social_API.Features.Chat.ChatHubs;
using Microsoft.AspNetCore.SignalR;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBT_Social_API.Src.Core.Helper;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using HUBT_Social_API.Features.Chat.Services.Interfaces;

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
        ReplyMessage replyMessage = null;

        if(chatRequest.ReplyToMessageId is not null)
        {
            MessageModel? messageResult = await RoomChatHelper.GetInfoMessageAsync(chatRequest.GroupId,chatRequest.ReplyToMessageId);
            if(messageResult is not null)
            {
                replyMessage = new ReplyMessage
                {
                    message = messageResult.message ?? null,
                    replyBy = chatRequest.UserId,
                    replyTo =messageResult.sentBy,
                    messageType = messageResult.messageType,
                    voiceMessageDuration = messageResult.voiceMessageDuration ?? null,
                    messageId = messageResult.id,
                };
            }
        }  
        // Gửi tin nhắn nếu có
        if (chatRequest.Content != null)
        {
            tasks.Add(Task.Run(async () =>
            {
                var messageRequest = new MessageRequest
                {
                    GroupId = chatRequest.GroupId,
                    Content = chatRequest.Content,
                    UserId = chatRequest.UserId,
                    ReplyToMessage = replyMessage
                };
                try { return await _messageUploadService.UploadMessageAsync(messageRequest, _hubContext); }
                catch(Exception exex) { 
                    Console.WriteLine("Gửi mess lỗi."+ exex.Message);
                    
                    return false; }
            }));
        }
            

        // Gửi media nếu có
        if (chatRequest.Medias is not null && chatRequest.Medias.Count >0 ) 
        {
            tasks.Add(Task.Run(async () =>
            {
                var mediaRequest = new MediaRequest
                {
                    GroupId = chatRequest.GroupId,
                    Medias = chatRequest.Medias.ConvertBase64sToFormFiles(),
                    UserId = chatRequest.UserId,
                    ReplyToMessage = replyMessage
                };
                try { return await _mediaUploadService.UploadMediaAsync(mediaRequest, _hubContext); }
                catch { 
                    Console.WriteLine("Gửi Media lỗi.");
                    return false; }
            }));
        }
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