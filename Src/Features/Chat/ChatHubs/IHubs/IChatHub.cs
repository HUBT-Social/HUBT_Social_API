using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
public interface IChatHub
{
    Task SendMessage(string GroupId, MessageChatItem messageModel);
    Task SendMedia(string chatRoomId,MediaChatItem mediaModels);
}
