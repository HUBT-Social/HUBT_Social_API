namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IFileService
{
    Task<bool> UploadFileAsync(string chatRoomId, string userId, IFormFile file);
}