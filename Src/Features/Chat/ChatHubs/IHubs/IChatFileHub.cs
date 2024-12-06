using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.ChatHubs.IHubs;

public interface IChatFileHub
{
    Task SendMedia(string chatRoomId,MediaChatItem mediaModels);
}