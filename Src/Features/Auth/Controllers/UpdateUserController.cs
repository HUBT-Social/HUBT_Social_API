using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Controllers;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UpdateUserController : ControllerBase
{
    protected readonly IEmailService _emailService;

    protected readonly ITokenService _tokenService;

    protected readonly IUserService _userService;

    public UpdateUserController(ITokenService tokenService, IEmailService emailService,IUserService userService)
    {
        _tokenService = tokenService;
        _emailService = emailService;
        _userService = userService;
    }

    [HttpGet("get-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success && userResponse.User != null)
        {
            return Ok(
                new
                {
                    AvatarUrl = userResponse.User.AvataUrl,
                    UserName = userResponse.User.UserName,
                    FirstName = userResponse.User.FirstName,
                    LastName = userResponse.User.LastName,
                    Gender = userResponse.User.Gender,
                    Email = userResponse.User.Email,
                    BirthDay = userResponse.User.DateOfBirth,
                    PhoneNumber = userResponse.User.PhoneNumber
                }
            );
        }

        return BadRequest(userResponse.Message);

    }
    [HttpGet("get-user-by-username")]
    public async Task<IActionResult> GetUserByUserName([FromQuery] GetUserByUserNameRequest getUserByUserNameRequest)
    {
        if(string.IsNullOrEmpty(getUserByUserNameRequest.UserName))
        {
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        }
        AUser? user = await _userService.FindUserByUserNameAsync(getUserByUserNameRequest.UserName);
        if (user != null)
        {
            return Ok(
                new
                {
                    AvatarUrl = user.AvataUrl,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    Email = user.Email,
                    BirthDay = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber
                }
            );
        }
        return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

    }
    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser(DeleteUserRequest request)
    {
        try
        {
            if(string.IsNullOrEmpty(request.UserName))
            {
                return BadRequest(LocalValue.Get(KeyStore.UsernameCannotBeEmpty));
            }

            AUser user = await _userService.FindUserByUserNameAsync(request.UserName);
            if(user == null)
            {
                return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
            }
                
            bool deleted = await _userService.DeleteUserAsync(user);
            if(deleted)
            {
                return Ok(LocalValue.Get(KeyStore.UserDeleted));
            }
            
        }
        catch (Exception)
        {
            return BadRequest(LocalValue.Get(KeyStore.UserDeletedError));
        }
        return BadRequest(LocalValue.Get(KeyStore.UserDeletedError));
    }

    

    [HttpPost("update/avatar")]
    public async Task<IActionResult> UpdateAvatar(IFormFile file)
    {
        if(file !=null){
            // Lấy thông tin người dùng từ token
            UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

            if (string.IsNullOrWhiteSpace(userResponse?.User?.UserName))
            {
                return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));
            }
            string foderPath = $"avatar/user/{userResponse.User.UserName}";
            string? avatarUrl = await UploadToStoreS3.CloudinaryService.UpdateAvatarAsync(foderPath,file);
            if(avatarUrl != null)
            {
                UpdateAvatarUrlRequest request = new();
                request.AvatarUrl = avatarUrl;
                return await UpdateHelper.HandleUserUpdate(KeyStore.AvatarUpdated, KeyStore.AvatarUpdateError, _userService.UpdateAvatarUrlAsync, request,Request,_tokenService);    
            }
            return BadRequest(LocalValue.Get(KeyStore.AvatarUpdateError));
        }
        return BadRequest(KeyStore.AvatarUpdateError);
    }
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserInfo(
        [FromQuery] string? email,
        [FromQuery] string? firstname,
        [FromQuery] string? lastname,
        [FromQuery] string? phoneNumber,
        [FromQuery] string? gender,
        [FromQuery] DateTime? dateOfBirth)
    {
        // Tạo đối tượng yêu cầu cập nhật chung
        GeneralUpdateRequest generalUpdateRequest = new GeneralUpdateRequest();
        
        if (!string.IsNullOrEmpty(email)) generalUpdateRequest.Email = email;
        if (!string.IsNullOrEmpty(firstname)) generalUpdateRequest.FirstName = firstname;
        if (!string.IsNullOrEmpty(lastname)) generalUpdateRequest.LastName = lastname;
        if (!string.IsNullOrEmpty(phoneNumber)) generalUpdateRequest.PhoneNumber = phoneNumber;
        if (!string.IsNullOrEmpty(gender))
        {
            if(gender !="male" || gender !="female") generalUpdateRequest.Gender = Gender.Other;
            if(gender=="male") generalUpdateRequest.Gender = Gender.Male;
            if(gender=="female") generalUpdateRequest.Gender = Gender.Female;

        }  // Sửa lỗi gán giá trị
        if (dateOfBirth != null) generalUpdateRequest.DateOfBirth = dateOfBirth;
        
        return await UpdateHelper.HandleUserUpdate(KeyStore.GeneralUpdateSuccess, KeyStore.GeneralUpdateError, _userService.GeneralUpdateAsync, generalUpdateRequest,Request,_tokenService);
    }
    
    [HttpPost("update/email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.EmailUpdated, KeyStore.EmailUpdateError, _userService.UpdateEmailAsync, request,Request,_tokenService);

    [HttpPost("update/password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        if(request.NewPassword == request.ConfirmNewPassword)
        {
            await UpdateHelper.HandleUserUpdate(KeyStore.PasswordUpdated, KeyStore.PasswordUpdateError, _userService.UpdatePasswordAsync, request,Request,_tokenService);
        }
        return BadRequest(LocalValue.Get(KeyStore.ConfirmPasswordError));
    }
        
    
    
    

    [HttpPut("two-factor-enable")]
    public async Task<IActionResult> EnableTwoFactor() =>
        await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError, (userName, _) => _userService.EnableTwoFactor(userName), new object(),Request,_tokenService);

    [HttpPut("two-factor-disable")]
    public async Task<IActionResult> DisableTwoFactor() =>
        await UpdateHelper.HandleUserUpdate(KeyStore.UserInfoUpdatedSuccess, KeyStore.UserInfoUpdateError, (userName, _) => _userService.DisableTwoFactor(userName), new object(),Request,_tokenService);

    
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
    
    [HttpPost("add-info-user")]
    public async Task<IActionResult> AddInfoUser(AddInfoUserRequest request)
    {
        
        // Gọi hàm xử lý cập nhật thông tin người dùng
        return await UpdateHelper.HandleUserUpdate(
            KeyStore.GeneralUpdateSuccess,
            KeyStore.GeneralUpdateError,
            _userService.AddInfoUser,
            request,
            Request,_tokenService
        );
    }
    
}
