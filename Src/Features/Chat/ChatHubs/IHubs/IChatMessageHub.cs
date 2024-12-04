using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.ChatHubs.IHubs;

public interface IChatMessageHub
{
    Task SendMessage(string GroupId, MessageModel messageModel);

}