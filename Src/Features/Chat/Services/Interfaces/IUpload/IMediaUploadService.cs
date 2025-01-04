
using HUBT_Social_API.Features.Chat.DTOs;
namespace HUBT_Social_API.Features.Chat.Services.Interfaces;
public interface IMediaUploadService
{
    Task<bool> UploadMediaAsync(MediaRequest chatRequest);
}