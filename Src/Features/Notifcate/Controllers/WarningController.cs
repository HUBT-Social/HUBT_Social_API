using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notation_API.Src.Services;
using MongoDB.Driver;
using HUBT_Social_API.Src.Features.Notifcate.Services;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Src.Features.Notifcate.Models.OutSourceDataDTO;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using Microsoft.AspNetCore.Authorization;

namespace HUBT_Social_API.Src.Features.Notifcate.Controllers
{
    [Route("api/notification/hubt")]
    [ApiController]
    [Authorize]
    public class WarningController(INotationService notationService,
        IFireBaseNotificationService fireBaseNotificationService,
        ITokenService tokenService
        ) : ControllerBase
    {
        private readonly INotationService _notationService = notationService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IFireBaseNotificationService _fireBaseNotificationService = fireBaseNotificationService;
        [HttpGet("check-score")]
        public async Task<IActionResult> GetAVGScoreByMasv(string masv)
        {
            UserResponse? userResponse = await TokenHelper.GetUserResponseFromToken(Request,_tokenService);
            if (userResponse == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            AVGScoreDTO? result = await _notationService.GetAVGScoreByMasv(masv);
            if (result == null)
                return NotFound();

            try
            {
                string? userFcm = userResponse.User?.FCMToken;
                if (userFcm == null)
                    return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
                int course = int.Parse(result.MaSV[..2]);
                int year = DateTime.Now.Month < 8 ? DateTime.Now.Year - 1994 + course - 1 : DateTime.Now.Year - 1994 + course;

                if (year > 3 && result.DiemTB4 < 2.2 || year == 2 && result.DiemTB4 < 2 || year == 1 && result.DiemTB4 < 1.8)
                {
                    SendMessageRequest message = new()
                    {
                        Token = userFcm,
                        Title = "Cảnh báo điểm số!!!",
                        Body = $"Sinh viên {result.MaSV} có điểm trung bình 10 là {result.DiemTB10}, điểm trung bình 4 là {result.DiemTB4}."
                    };
                    await _fireBaseNotificationService.SendPushNotificationToOneAsync(message);
                }
                else
                {
                    SendMessageRequest message = new()
                    {
                        Token = userFcm,
                        Title = "điểm số của bạn",
                        Body = $"Sinh viên {result.MaSV} có điểm trung bình 10 là {result.DiemTB10}, điểm trung bình 4 là {result.DiemTB4}."
                    };
                    await _fireBaseNotificationService.SendPushNotificationToOneAsync(message);
                }

                return Ok(LocalValue.Get(KeyStore.MessageSentSuccessfully));

            }
            catch (Exception)
            {
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            }
        }


        //[HttpGet("get-student")]
        //public async Task<IActionResult> GetStudentByMasv(string masv)
        //{
        //    StudentDTO? result = await _notationService.GetStudentByMasv(masv);
        //    return result != null ? Ok(result) : NotFound();
        //}
        //[HttpGet("get-student-score")]
        //public async Task<IActionResult> GetStudentScoreByMasv(string masv)
        //{
        //    List<ScoreDTO>? result = await _notationService.GetStudentScoreByMasv(masv);
        //    return result != null ? Ok(result) : NotFound();
        //}
        //[HttpGet("get-time-table")]
        //public async Task<IActionResult> GetTimeTableByClassName(string className)
        //{
        //    List<TimeTableDTO>? result = await _notationService.GetTimeTableByClassName(className);
        //    return result != null ? Ok(result) : NotFound();
        //}
    }
}
