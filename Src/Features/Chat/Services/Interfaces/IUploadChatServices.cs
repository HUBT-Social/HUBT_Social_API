using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IUploadChatServices
{
    Task<bool> SendMessageAsync(MessageRequest chatRequest,string eventName);
    Task<bool> SendMediaAsync(MediaRequest chatRequest,string eventName);
    Task<bool> SendFileAsync(IFormFile file);

}