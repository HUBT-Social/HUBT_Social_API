using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.Services;

public class ChatService : IChatService
{
    private readonly IUploadChatServices _uploadtService;


    public ChatService(IUploadChatServices uploadtService)
    {
        _uploadtService = uploadtService;
    }

    //Send message (Text,icon,images,file)
    public async Task<bool> SendMessageAsync(MessageRequest messageRequest)
    {
        return await _uploadtService.UploadMessageAsync(messageRequest);
    }

    public async Task<bool> SendMediaAsync(FileRequest chatRequest)
    {
        return await _uploadtService.UploadMediaAsync(chatRequest);
    }
    //Unsend message

    //Delete message

    //...

    
}