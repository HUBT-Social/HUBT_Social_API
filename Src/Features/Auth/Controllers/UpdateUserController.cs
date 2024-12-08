using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Controllers;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
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
public class UpdateUserController : BaseAuthController
{
    private readonly IImageService _imageService;
    public UpdateUserController(ITokenService tokenService, IEmailService emailService,IUserService userService,IImageService imageService)
    :base (null,tokenService,emailService,null,userService)
    {
        _imageService = imageService;
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

    [HttpPost("get-url-from-image")]
    public async Task<IActionResult> GetUrlFromImage(IFormFile file)
    {
        string avatarUrl;
        try
        {
            avatarUrl = await _imageService.GetUrlFormFileAsync(file);
        }
        catch (Exception)
        {
            return BadRequest(
                new
                {
                    Success = false,
                    Data = $"{LocalValue.Get(KeyStore.InvalidFileData)}"
                });
        }
        if (avatarUrl != null)
        {
            return Ok(
                new
                {
                    Success = true,
                    Data = avatarUrl
                });
        }
        return BadRequest(
            new
            {
                Success = false,
                Data = $"{LocalValue.Get(KeyStore.InvalidFileData)}"
            }
        );
    }

    [HttpPost("update/avatar")]
    public async Task<IActionResult> UpdateAvatar(IFormFile file)
    {
        if(file !=null){
            string avatarUrl;
            try
            {
                avatarUrl = await _imageService.GetUrlFormFileAsync(file); 
            }
            catch (Exception ex)
            {
                return BadRequest($"{LocalValue.Get(KeyStore.InvalidFileData)}");
            }
            UpdateAvatarUrlRequest request = new();
            request.AvatarUrl = avatarUrl;
            return await UpdateHelper.HandleUserUpdate(KeyStore.AvatarUpdated, KeyStore.AvatarUpdateError, _userService.UpdateAvatarUrlAsync, request,Request,_tokenService);
        }
        return BadRequest(KeyStore.AvatarUpdateError);
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
        

    [HttpPost("update/name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.NameUpdated, KeyStore.NameUpdateError, _userService.UpdateNameAsync, request,Request,_tokenService);

    [HttpPost("update/phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.PhoneNumberUpdated, KeyStore.PhoneNumberUpdateError, _userService.UpdatePhoneNumberAsync, request,Request,_tokenService);

    [HttpPost("update/gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.GenderUpdated, KeyStore.GenderUpdateError, _userService.UpdateGenderAsync, request,Request,_tokenService);

    [HttpPost("update/date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.DateOfBirthUpdated, KeyStore.DateOfBirthUpdateError, _userService.UpdateDateOfBirthAsync, request,Request,_tokenService);

    [HttpPost("update/general")]
    public async Task<IActionResult> GeneralUpdate(GeneralUpdateRequest request) =>
        await UpdateHelper.HandleUserUpdate(KeyStore.GeneralUpdateSuccess, KeyStore.GeneralUpdateError, _userService.GeneralUpdateAsync, request,Request,_tokenService);
    
    

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
            // Kiểm tra file upload
        if (request.AvatarUrl == null)
        {
            request.AvatarUrl = KeyStore.GetRandomAvatarDefault(request.Gender);  
        }

        
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
