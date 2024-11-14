using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/user-update")]
[Authorize]
public class UpdateUserController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    public UpdateUserController(IUserService userService, IEmailService emailService, ITokenService tokenService)
    {
        _userService = userService;
        _emailService = emailService;
        _tokenService = tokenService;
    }

    [HttpGet("get-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success && !string.IsNullOrWhiteSpace(userResponse.Username))
        {
            AUser? user = await _userService.FindUserByUserNameAsync(userResponse.Username);
            return Ok(user);
        }
        
        return BadRequest(LocalValue.Get(KeyStore.EmailCannotBeEmpty));

        
    }
    [HttpPost("promote")]
    public async Task<IActionResult> PromoteUserAccount([FromBody] PromoteUserRequest request)
    {
        // Extract token and get current user information
        var userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.Username))
        {
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        }

        // Validate request data
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.RoleName))
        {
            return BadRequest("Invalid request data.");
        }

        // Attempt to promote the target user
        var result = await _userService.PromoteUserAccountAsync(userResponse.Username, request.UserName, request.RoleName);

        if (result)
        {
            return Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess));
        }
        else
        {
            return BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
        }
    }
    

    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(LocalValue.Get(KeyStore.EmailCannotBeEmpty));

        var result = await _userService.UpdateEmailAsync(userResponse.Username,request);
        return result ? Ok(LocalValue.Get(KeyStore.EmailUpdated)) : BadRequest(LocalValue.Get(KeyStore.EmailUpdateError));
    }

    // Cập nhật mật khẩu
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(LocalValue.Get(KeyStore.PasswordCannotBeEmpty));
            

        var result = await _userService.UpdatePasswordAsync(userResponse.Username, request);
        return result ? Ok(LocalValue.Get(KeyStore.PasswordUpdated)) : BadRequest(LocalValue.Get(KeyStore.PasswordUpdateError));
    }

    // Cập nhật tên người dùng
    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(LocalValue.Get(KeyStore.UsernameCannotBeEmpty));

        var result = await _userService.UpdateNameAsync(userResponse.Username, request);
        return result ? Ok(LocalValue.Get(KeyStore.NameUpdated)) : BadRequest(LocalValue.Get(KeyStore.NameUpdateError));
    }

    // Cập nhật số điện thoại
    [HttpPost("update-phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.UpdatePhoneNumberAsync(userResponse.Username,request);
        return result ? Ok(LocalValue.Get(KeyStore.PhoneNumberUpdated)) : BadRequest(LocalValue.Get(KeyStore.PhoneNumberUpdateError));
    }

    // Cập nhật giới tính
    [HttpPost("update-gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.UpdateGenderAsync(userResponse.Username, request);
        return result ? Ok(LocalValue.Get(KeyStore.GenderUpdated)) : BadRequest(LocalValue.Get(KeyStore.GenderUpdateError));
    }

    // Cập nhật ngày sinh
    [HttpPost("update-date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.UpdateDateOfBirthAsync(userResponse.Username, request);
        return result ? Ok(LocalValue.Get(KeyStore.DateOfBirthUpdated)) : BadRequest(LocalValue.Get(KeyStore.DateOfBirthUpdateError));
    }

    // Cập nhật thông tin người dùng chung
    [HttpPost("general-update")]
    public async Task<IActionResult> GeneralUpdate([FromBody] GeneralUpdateRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.GeneralUpdateAsync(userResponse.Username,request);
        return result ? Ok(LocalValue.Get(KeyStore.GeneralUpdateSuccess)) : BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
    }

    [HttpPut("two-factor-enable")]
    public async Task<IActionResult> EnableTwoFactor()
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);


        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        bool result = await _userService.EnableTwoFactor(userResponse.Username);
        return result ? Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess)) : BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
    }
    [HttpPut("two-factor-disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        bool result = await _userService.DisableTwoFactor(userResponse.Username);
        return result ? Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess)) : BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
    }

}
