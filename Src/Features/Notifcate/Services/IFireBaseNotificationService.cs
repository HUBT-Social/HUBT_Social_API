using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;

namespace HUBT_Social_API.Src.Features.Notifcate.Services;

public interface IFireBaseNotificationService
{
    Task SendPushNotificationToOneAsync(SendMessageRequest request);
    Task SendPushNotificationToManyAsync(SendGroupMessageRequest request);
}