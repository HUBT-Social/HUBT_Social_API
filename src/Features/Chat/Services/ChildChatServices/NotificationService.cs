namespace HUBTSOCIAL.Src.Features.Chat.Services.IChatServices
{
    public class NotificationService
    {
        public async Task SendNotification(string userId, string message)
        {
            // Code to send push notification (Firebase, etc.)
            await Task.CompletedTask;
        }
    }
}
