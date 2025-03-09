using FirebaseAdmin.Messaging;
using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using MongoDB.Bson;

namespace HUBT_Social_API.Src.Features.Notifcate.Services;

public class FireBaseNotificationService : IFireBaseNotificationService
{
    public async Task SendPushNotificationToOneAsync(SendMessageRequest request)
    {
        await SendNotificationAsync(request);
    }
    private static async Task SendNotificationAsync(MessageRequest request)
    {
        Message message;
        if (request is SendMessageRequest sendToOne)
        {
            message = new Message
            { 
                Token = sendToOne.Token,
                Notification = new Notification
                {
                    Title = sendToOne.Title,
                    Body = sendToOne.Body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", sendToOne.Type }
                }
            };

        }
        else if (request is SendGroupMessageRequest sendToMany)
        {
            message = new Message
            {
                Topic = sendToMany.GroupId,
                Notification = new Notification
                {
                    Title = sendToMany.Title,
                    Body = sendToMany.Body
                },
                Data = new Dictionary<string, string>
                {
                    { "type", sendToMany.Type }
                }
            };
        }
        else
        {
            throw new Exception("Invalid request type");
        }
        try {  
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Successfully sent message: {message.ToJson()}");
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }

    public async Task SendPushNotificationToManyAsync(SendGroupMessageRequest request)
    {
        await SendNotificationAsync(request);
    }
}