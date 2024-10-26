namespace HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs
{
    public interface IChatImageHub
    {
        Task SendImage(string chatRoomId, string userId, byte[] imageData);
    }    
}
