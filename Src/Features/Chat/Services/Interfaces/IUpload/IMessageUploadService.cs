
using HUBT_Social_API.Features.Chat.DTOs;
namespace HUBT_Social_API.Features.Chat.Services.Interfaces;
public interface IMessageUploadService
{
    Task<bool> UploadMessageAsync(MessageRequest chatRequest);
}