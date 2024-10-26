
namespace HUBTSOCIAL.Src.Features.Chat.Services.IChatServices
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(string chatRoomId, byte[] fileData, string fileName);
    }
}