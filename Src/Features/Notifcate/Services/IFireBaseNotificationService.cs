using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;

namespace HUBT_Social_API.Src.Features.Notifcate.Services
{
    public interface IFireBaseNotificationService
    {
        Task SendPushNotificationAsync(SendMessageRequest request);
    }
}