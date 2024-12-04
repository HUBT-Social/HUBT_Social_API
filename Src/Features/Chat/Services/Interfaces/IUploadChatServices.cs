using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IUploadChatServices
{
    Task<bool> UploadMessageAsync(MessageRequest chatRequest);
    Task<bool> UploadMediaAsync(FileRequest chatRequest);
    Task<string> UploadToStorageAsync(IFormFile file);
}