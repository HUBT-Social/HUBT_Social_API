using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IChatService
{
    Task<bool> SendMessageAsync(MessageRequest chatRequest);
    Task<bool> SendFileAsync(FileRequest chatRequest);
    
}