using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.Services;

public class ChatService //: IChatService
{
    private readonly IUploadServices _uploadtService;


    public ChatService(IUploadServices uploadtService)
    {
        _uploadtService = uploadtService;
    }


    public async Task<bool> SendMessageAsync(ChatRequest chatRequest)
    {
        return await _uploadtService.UploadChatAsync(chatRequest);
    }


    
}