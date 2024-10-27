namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IImageService
{
    Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData);
}