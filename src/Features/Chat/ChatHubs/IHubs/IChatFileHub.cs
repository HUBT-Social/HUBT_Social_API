namespace HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs
{
    public interface IChatFileHub
    {
        Task SendFile(string chatRoomId, string userId, byte[] fileData, string fileName);
    }    
}
