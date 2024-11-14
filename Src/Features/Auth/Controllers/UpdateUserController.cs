using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse != null && !string.IsNullOrWhiteSpace(userResponse.UserName) )
        {
            AUser? user = await _userService.FindUserByUserNameAsync(userResponse.UserName);
            return Ok(user);
        }
        
        return BadRequest(LocalValue.Get(KeyStore.EmailCannotBeEmpty));

        
    }

    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);

        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(LocalValue.Get(KeyStore.EmailCannotBeEmpty));

        var result = await _userService.UpdateEmailAsync(userResponse.UserName,request);
        return result ? Ok(LocalValue.Get(KeyStore.EmailUpdated)) : BadRequest(LocalValue.Get(KeyStore.EmailUpdateError));
    }

    // Cập nhật mật khẩu
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(LocalValue.Get(KeyStore.PasswordCannotBeEmpty));
            

        var result = await _userService.UpdatePasswordAsync(userResponse.UserName, request);
        return result ? Ok(LocalValue.Get(KeyStore.PasswordUpdated)) : BadRequest(LocalValue.Get(KeyStore.PasswordUpdateError));
    }

    // Cập nhật tên người dùng
    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(LocalValue.Get(KeyStore.UsernameCannotBeEmpty));

        var result = await _userService.UpdateNameAsync(userResponse.UserName, request);
        return result ? Ok(LocalValue.Get(KeyStore.NameUpdated)) : BadRequest(LocalValue.Get(KeyStore.NameUpdateError));
    }

    // Cập nhật số điện thoại
    [HttpPost("update-phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName) || string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.UpdatePhoneNumberAsync(userResponse.UserName,request);
        return result ? Ok(LocalValue.Get(KeyStore.PhoneNumberUpdated)) : BadRequest(LocalValue.Get(KeyStore.PhoneNumberUpdateError));
    }

    // Cập nhật giới tính
    [HttpPost("update-gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.UpdateGenderAsync(userResponse.UserName, request);
        return result ? Ok(LocalValue.Get(KeyStore.GenderUpdated)) : BadRequest(LocalValue.Get(KeyStore.GenderUpdateError));
    }

    // Cập nhật ngày sinh
    [HttpPost("update-date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.UpdateDateOfBirthAsync(userResponse.UserName, request);
        return result ? Ok(LocalValue.Get(KeyStore.DateOfBirthUpdated)) : BadRequest(LocalValue.Get(KeyStore.DateOfBirthUpdateError));
    }

    // Cập nhật thông tin người dùng chung
    [HttpPost("general-update")]
    public async Task<IActionResult> GeneralUpdate([FromBody] GeneralUpdateRequest request)
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);
        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        var result = await _userService.GeneralUpdateAsync(userResponse.UserName,request);
        return result ? Ok(LocalValue.Get(KeyStore.GeneralUpdateSuccess)) : BadRequest(LocalValue.Get(KeyStore.GeneralUpdateError));
    }

    [HttpPut("two-factor-enable")]
    public async Task<IActionResult> EnableTwoFactor()
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);

        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        bool result = await _userService.EnableTwoFactor(userResponse.UserName);
        return result ? Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess)) : BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
    }
    [HttpPut("two-factor-disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        AUser? userResponse = await _tokenService.GetCurrentUser(token);

        if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.UserName))
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));


        bool result = await _userService.DisableTwoFactor(userResponse.UserName);
        return result ? Ok(LocalValue.Get(KeyStore.UserInfoUpdatedSuccess)) : BadRequest(LocalValue.Get(KeyStore.UserInfoUpdateError));
    }

}
