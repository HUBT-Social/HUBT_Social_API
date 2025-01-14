using FirebaseAdmin.Messaging;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using MongoDB.Bson;

namespace HUBT_Social_API.Src.Features.Notifcate.Services
{
    public class FireBaseNotificationService : IFireBaseNotificationService
    {
        public async Task SendPushNotificationAsync(SendMessageRequest request)
        {
            var message = new Message()
            {
                Token = request.Token,
                Notification = new Notification()
                {
                    Title = request.Title,
                    Body = request.Body,
                },
                Data = new Dictionary<string, string>
                {
                    { "type", "chat" }
                }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Successfully sent message: {message.ToJson()}");
        }
    }
}
