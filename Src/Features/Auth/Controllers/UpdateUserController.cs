using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Services.Child;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using HUBT_Social_API.Src.Features.Notifcate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UpdateUserController : ControllerBase
{
    protected readonly IEmailService _emailService;

    protected readonly ITokenService _tokenService;

    protected readonly IUserService _userService;
    private readonly IFireBaseNotificationService _fireBaseNotificationService;

    public UpdateUserController(ITokenService tokenService, IEmailService emailService, IUserService userService, IFireBaseNotificationService fireBaseNotificationService)
    {
        _fireBaseNotificationService = fireBaseNotificationService;
        _tokenService = tokenService;
        _emailService = emailService;
        _userService = userService;
    }

    [HttpGet("get-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success && userResponse.User != null)
            return Ok(
                new
                {
                    AvatarUrl = userResponse.User.AvataUrl,
                    userResponse.User.UserName,
                    userResponse.User.FirstName,
                    userResponse.User.LastName,
                    userResponse.User.Gender,
                    userResponse.User.Email,
                    BirthDay = userResponse.User.DateOfBirth,
                    PhoneNumber = userResponse.User.PhoneNumber,
                    Roles = await _userService.ConvertIdRoleToNameAsync(userResponse.User.Roles)
                }
            );

        return BadRequest(userResponse.Message);
    }

    [HttpGet("get-user-by-username")]
    public async Task<IActionResult> GetUserByUserName([FromQuery] GetUserByUserNameRequest getUserByUserNameRequest)
    {
        if (string.IsNullOrEmpty(getUserByUserNameRequest.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        var user = await _userService.FindUserByUserNameAsync(getUserByUserNameRequest.UserName);
        if (user != null)
            return Ok(
                new
                {
                    AvatarUrl = user.AvataUrl,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    user.Gender,
                    user.Email,
                    BirthDay = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    Roles = await _userService.ConvertIdRoleToNameAsync(user.Roles)
                }
            );
        return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser(DeleteUserRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserName))
                return BadRequest(LocalValue.Get(KeyStore.UsernameCannotBeEmpty));

            var user = await _userService.FindUserByUserNameAsync(request.UserName);
            if (user == null) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

            var deleted = await _userService.DeleteUserAsync(user);
            if (deleted) return Ok(LocalValue.Get(KeyStore.UserDeleted));
        }
        catch (Exception)
        {
            return BadRequest(LocalValue.Get(KeyStore.UserDeletedError));
        }

        return BadRequest(LocalValue.Get(KeyStore.UserDeletedError));
    }


    [HttpPut("update/avatar")]
    public async Task<IActionResult> UpdateAvatar(IFormFile file)
    {
        if (file != null)
        {
            // Lấy thông tin người dùng từ token
            var userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

            if (string.IsNullOrWhiteSpace(userResponse?.User?.UserName))
                return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));
            var foderPath = $"avatar/user/{userResponse.User.UserName}";
            var avatarUrl = await UploadToStoreS3.CloudinaryService.UpdateAvatarAsync(foderPath, file);
            if (avatarUrl != null)
            {
                UpdateAvatarUrlRequest request = new();
                request.AvatarUrl = avatarUrl;
                return await UpdateHelper.HandleUserUpdate(KeyStore.AvatarUpdated, KeyStore.AvatarUpdateError,
                    _userService.UpdateAvatarUrlAsync, request, Request, _tokenService);
            }

            return BadRequest(LocalValue.Get(KeyStore.AvatarUpdateError));
        }

        return BadRequest(KeyStore.AvatarUpdateError);
    }
    [HttpPut("update-general")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] GeneralUpdateRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.GeneralUpdateSuccess, KeyStore.GeneralUpdateError, _userService.GeneralUpdateAsync, request,Request,_tokenService);
    
    [HttpPost("update/email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        return await UpdateHelper.HandleUserUpdate(KeyStore.EmailUpdated, KeyStore.EmailUpdateError,
            _userService.UpdateEmailAsync, request, Request, _tokenService);
    }

    [HttpPut("update/password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        if (request.NewPassword == request.ConfirmNewPassword)
            await UpdateHelper.HandleUserUpdate(KeyStore.PasswordUpdated, KeyStore.PasswordUpdateError,
                _userService.UpdatePasswordAsync, request, Request, _tokenService);
        return BadRequest(LocalValue.Get(KeyStore.ConfirmPasswordError));
    }

    [HttpPut("two-factor-enable")]
    public async Task<IActionResult> EnableTwoFactor()
    {
        return await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError,
            (userName, _) => _userService.EnableTwoFactor(userName), new object(), Request, _tokenService);
    }

    [HttpPut("two-factor-disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
        return await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError,
            (userName, _) => _userService.DisableTwoFactor(userName), new object(), Request, _tokenService);
    }

    [HttpPut("update/status")]
    public async Task<IActionResult> UpdateStatus(UpdateStatusRequest request)
    {
        return await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError,
            (userName, _) => _userService.UpdateStatusAsync(userName, request.Bio), new object(), Request,
            _tokenService);
    }

    [HttpPut("update/fcm-token")]
    public async Task<IActionResult> StoreFcm(StoreFCMRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);


        if (string.IsNullOrWhiteSpace(userResponse?.User?.UserName))
            return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));

        if (userResponse.User.FCMToken != request.FcmToken && !string.IsNullOrEmpty(userResponse.User.FCMToken))
            await _fireBaseNotificationService.SendPushNotificationAsync(new SendMessageRequest { Token = userResponse.User.FCMToken, Title = "Thông báo", Body = "Bạn đã đăng nhập ở một thiết bị khác" });
        return await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError,
            (userName, _) => _userService.UpdateFcmTokenAsync(userName, request.FcmToken), new object(), Request,
            _tokenService);
    }
    [HttpPut("Delete/fcm-token")]
    public async Task<IActionResult> DeleteStoreFcm()
    {

        return await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError,
            (userName, _) => _userService.UpdateStatusAsync(userName,""), new object(), Request,
            _tokenService);
    }


    [HttpPost("promote")]
    public async Task<IActionResult> PromoteUserAccount([FromBody] PromoteUserRequest request)
    {
        // Validate request data
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.RoleName))
            return BadRequest("Invalid request data.");

        // Extract token and get current user information
        var userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (!userResponse.Success) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

        // Attempt to promote the target user
        var result =
            await _userService.PromoteUserAccountAsync(userResponse.User.UserName, request.UserName, request.RoleName);

        if (result) return Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess));

        return BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
    }

    [HttpPut("add-info-user")]
    public async Task<IActionResult> AddInfoUser(AddInfoUserRequest request)
    {
        // Gọi hàm xử lý cập nhật thông tin người dùng
        return await UpdateHelper.HandleUserUpdate(
            KeyStore.GeneralUpdateSuccess,
            KeyStore.GeneralUpdateError,
            _userService.AddInfoUser,
            request,
            Request, _tokenService
        );
    }

    [HttpPost("store-UserPromotion")]
    public async Task<IActionResult> StoreUserPromotion(PromotedStoreRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        var userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success && userResponse.User != null)
        {
            var user = userResponse.User;
            return await _userService.StoreUserPromotionAsync(user.Id.ToString(), user.Email, request)
                ? Ok(LocalValue.Get(KeyStore.StoreSuccessfull))
                : BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        }

        return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
    }
}