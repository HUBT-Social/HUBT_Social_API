using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using HUBT_Social_API.Src.Features.Notifcate.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Src.Features.Notifcate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendNotificationController : ControllerBase
    {
        private readonly IFireBaseNotificationService _notification;
        
        public SendNotificationController(IFireBaseNotificationService notification)
        {
            _notification = notification;
        }

        [HttpPost("send-Notification")]
        public async Task<IActionResult> SendNotification([FromBody] SendMessageRequest request)
        {
            try
            {
                await _notification.SendPushNotificationAsync(request);
                return Ok();
            }catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return BadRequest();
            }
        }

    }
}
