using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using HUBT_Social_API.Src.Features.Notifcate.Services;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Src.Features.Notifcate.Controllers;

[Route("api/notification")]
[ApiController]
public class SendNotificationController : ControllerBase
{
    private readonly IFireBaseNotificationService _notification;

    public SendNotificationController(IFireBaseNotificationService notification)
    {
        _notification = notification;
    }

    [HttpPost("send-notification-to-one")]
    public async Task<IActionResult> SendNotification([FromBody] SendMessageRequest request)
    {
        try
        {
            await _notification.SendPushNotificationToOneAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            return BadRequest();
        }
    }
    [HttpPost("send-notification-to-many")]
    public async Task<IActionResult> SendNotificationToMany([FromBody] SendGroupMessageRequest request)
    {
        try
        {
            await _notification.SendPushNotificationToManyAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            return BadRequest();
        }
    }
}