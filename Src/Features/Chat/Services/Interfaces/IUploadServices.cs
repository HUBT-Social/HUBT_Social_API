using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IUploadServices
{
    Task<bool> UploadMessageAsync(MessageRequest chatRequest);
    Task<bool> UploadFileAsync(FileRequest chatRequest);
    Task<string> UploadToStorageAsync(IFormFile file);
}