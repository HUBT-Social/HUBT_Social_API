using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/user-update")]
[Authorize]
public class UpdateUserController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITokenService _tokenService;
    public UpdateUserController(IUserService userService, IEmailService emailService, IStringLocalizer<SharedResource> localizer,ITokenService tokenService)
    {
        _userService = userService;
        _emailService = emailService;
        _localizer = localizer;
        _tokenService = tokenService;
    }

    // Cập nhật email
    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);

        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(_localizer["EmailCannotBeEmpty"].Value);

        var result = await _userService.UpdateEmailAsync(userResponse.Username,request);
        return result ? Ok(_localizer["EmailUpdated"].Value) : BadRequest(_localizer["EmailUpdateError"].Value);
    }

    // Cập nhật mật khẩu
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(_localizer["PasswordCannotBeEmpty"].Value);
            

        var result = await _userService.UpdatePasswordAsync(userResponse.Username, request);
        return result ? Ok(_localizer["PasswordUpdated"].Value) : BadRequest(_localizer["PasswordUpdateError"].Value);
    }

    // Cập nhật tên người dùng
    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(_localizer["UsernameCannotBeEmpty"].Value);

        var result = await _userService.UpdateNameAsync(userResponse.Username, request);
        return result ? Ok(_localizer["NameUpdated"].Value) : BadRequest(_localizer["NameUpdateError"].Value);
    }

    // Cập nhật số điện thoại
    [HttpPost("update-phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest(_localizer["UserNotFound"].Value);


        var result = await _userService.UpdatePhoneNumberAsync(userResponse.Username,request);
        return result ? Ok(_localizer["PhoneNumberUpdated"].Value) : BadRequest(_localizer["PhoneNumberUpdateError"].Value);
    }

    // Cập nhật giới tính
    [HttpPost("update-gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(_localizer["UserNotFound"].Value);


        var result = await _userService.UpdateGenderAsync(userResponse.Username, request);
        return result ? Ok(_localizer["GenderUpdated"].Value) : BadRequest(_localizer["GenderUpdateError"].Value);
    }

    // Cập nhật ngày sinh
    [HttpPost("update-date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(_localizer["UserNotFound"].Value);


        var result = await _userService.UpdateDateOfBirthAsync(userResponse.Username, request);
        return result ? Ok(_localizer["DateOfBirthUpdated"].Value) : BadRequest(_localizer["DateOfBirthUpdateError"].Value);
    }

    // Cập nhật thông tin người dùng chung
    [HttpPost("general-update")]
    public async Task<IActionResult> GeneralUpdate([FromBody] GeneralUpdateRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(_localizer["UserNotFound"].Value);


        var result = await _userService.GeneralUpdateAsync(userResponse.Username,request);
        return result ? Ok(_localizer["GeneralUpdateSuccess"].Value) : BadRequest(_localizer["GeneralUpdateError"].Value);
    }

    [HttpPut("two-factor-enable")]
    public async Task<IActionResult> EnableTwoFactor()
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);

        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(_localizer["UserNotFound"].Value);


        bool result = await _userService.EnableTwoFactor(userResponse.Username);
        return result ? Ok(_localizer["UserInfoUpdatedSuccess"].Value) : BadRequest(_localizer["UserInfoUpdateError"].Value);
    }
    [HttpPut("two-factor-disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);

        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(_localizer["UserNotFound"].Value);


        bool result = await _userService.DisableTwoFactor(userResponse.Username);
        return result ? Ok(_localizer["UserInfoUpdatedSuccess"].Value) : BadRequest(_localizer["UserInfoUpdateError"].Value);
    }

}
