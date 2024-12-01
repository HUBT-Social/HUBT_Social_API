using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IUploadServices
{
    Task<bool> UploadChatAsync(ChatRequest chatRequest);
    Task<string> UploadToStorageAsync(IFormFile file);
}