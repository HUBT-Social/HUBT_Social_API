using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Controllers;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/user-update")]
[Authorize]
public class UpdateUserController : BaseAuthController
{
    private readonly IImageService _imageService;
    public UpdateUserController(ITokenService tokenService, IEmailService emailService,IUserService userService,IImageService imageService)
    :base (null,tokenService,emailService,null,userService)
    {
        _imageService = imageService;
    }
    // Phương thức trợ giúp chung
    private async Task<IActionResult> HandleUserUpdate<TRequest>(string successMessage, string errorMessage, Func<string, TRequest, Task<bool>> updateFunc, TRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (string.IsNullOrWhiteSpace(userResponse.User.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

        var result = await updateFunc(userResponse.User.UserName, request);
        return result ? Ok(LocalValue.Get(successMessage)) : BadRequest(LocalValue.Get(errorMessage));
    }
    [HttpPost("update-avatar-url")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarUrlRequest request) =>
        await HandleUserUpdate(KeyStore.AvatarUpdated, KeyStore.AvatarUpdateError, _userService.UpdateAvatarUrlAsync, request);
    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request) =>
        await HandleUserUpdate(KeyStore.EmailUpdated, KeyStore.EmailUpdateError, _userService.UpdateEmailAsync, request);

    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request) =>
        await HandleUserUpdate(KeyStore.PasswordUpdated, KeyStore.PasswordUpdateError, _userService.UpdatePasswordAsync, request);

    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request) =>
        await HandleUserUpdate(KeyStore.NameUpdated, KeyStore.NameUpdateError, _userService.UpdateNameAsync, request);

    [HttpPost("update-phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request) =>
        await HandleUserUpdate(KeyStore.PhoneNumberUpdated, KeyStore.PhoneNumberUpdateError, _userService.UpdatePhoneNumberAsync, request);

    [HttpPost("update-gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request) =>
        await HandleUserUpdate(KeyStore.GenderUpdated, KeyStore.GenderUpdateError, _userService.UpdateGenderAsync, request);

    [HttpPost("update-date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request) =>
        await HandleUserUpdate(KeyStore.DateOfBirthUpdated, KeyStore.DateOfBirthUpdateError, _userService.UpdateDateOfBirthAsync, request);

    [HttpPost("general-update")]
    public async Task<IActionResult> GeneralUpdate([FromBody] GeneralUpdateRequest request) =>
        await HandleUserUpdate(KeyStore.GeneralUpdateSuccess, KeyStore.GeneralUpdateError, _userService.GeneralUpdateAsync, request);

    [HttpPut("two-factor-enable")]
    public async Task<IActionResult> EnableTwoFactor() =>
        await HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError, (userName, _) => _userService.EnableTwoFactor(userName), new object());

    [HttpPut("two-factor-disable")]
    public async Task<IActionResult> DisableTwoFactor() =>
        await HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError, (userName, _) => _userService.DisableTwoFactor(userName), new object());

    
    [HttpPost("promote")]
    public async Task<IActionResult> PromoteUserAccount([FromBody] PromoteUserRequest request)
    {
        // Validate request data
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.RoleName))
        {
            return BadRequest("Invalid request data.");
        }

        // Extract token and get current user information
        var userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (!userResponse.Success)
        {
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        }

        // Attempt to promote the target user
        var result = await _userService.PromoteUserAccountAsync(userResponse.User.UserName, request.UserName, request.RoleName);

        if (result)
        {
            return Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess));
        }
        else
        {
            return BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
        }
    }
    
    [HttpGet("add-info-user")]
    public async Task<IActionResult> AddInfoUser([FromForm] AddInfoUserRequest request)
    {
            // Kiểm tra file upload
        if (request.file != null || request.file.Length != 0)
        {
            // Upload ảnh lên Cloudinary
            string avatarUrl;
            try
            {
                avatarUrl = await _imageService.GetUrlFormFileAsync(request.file); 
            }
            catch (Exception ex)
            {
                return BadRequest($"{LocalValue.Get(KeyStore.InvalidFileData)}");
            }

            // Gán URL ảnh vào request
            request.AvatarUrl = avatarUrl;  
        }

        request.AvatarUrl = KeyStore.GetRandomAvatarDefault(request.Gender);

        // Gọi hàm xử lý cập nhật thông tin người dùng
        return await HandleUserUpdate(
            KeyStore.GeneralUpdateSuccess,
            KeyStore.GeneralUpdateError,
            _userService.AddInfoUser,
            request
        );
    }
}
