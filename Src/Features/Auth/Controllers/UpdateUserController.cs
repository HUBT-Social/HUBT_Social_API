using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UpdateUserController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public UpdateUserController(IUserService userService, IEmailService emailService, IStringLocalizer<SharedResource> localizer)
    {
        _userService = userService;
        _emailService = emailService;
        _localizer = localizer;
    }

    // Cập nhật email
    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(_localizer["EmailEmptyError"]);

        var result = await _userService.UpdateEmailAsync(request);
        return result ? Ok(_localizer["EmailUpdatedSuccess"]) : BadRequest(_localizer["EmailUpdateError"]);
    }

    // Cập nhật mật khẩu
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(_localizer["PasswordEmptyError"]);

        var result = await _userService.UpdatePasswordAsync(request);
        return result ? Ok(_localizer["PasswordUpdatedSuccess"]) : BadRequest(_localizer["PasswordUpdateError"]);
    }

    // Cập nhật tên người dùng
    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
    {
        var result = await _userService.UpdateNameAsync(request);
        return result ? Ok(_localizer["NameUpdatedSuccess"]) : BadRequest(_localizer["NameUpdateError"]);
    }

    // Cập nhật số điện thoại
    [HttpPost("update-phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
    {
        var result = await _userService.UpdatePhoneNumberAsync(request);
        return result ? Ok(_localizer["PhoneNumberUpdatedSuccess"]) : BadRequest(_localizer["PhoneNumberUpdateError"]);
    }

    // Cập nhật giới tính
    [HttpPost("update-gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request)
    {
        var result = await _userService.UpdateGenderAsync(request);
        return result ? Ok(_localizer["GenderUpdatedSuccess"]) : BadRequest(_localizer["GenderUpdateError"]);
    }

    // Cập nhật ngày sinh
    [HttpPost("update-date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request)
    {
        var result = await _userService.UpdateDateOfBirthAsync(request);
        return result ? Ok(_localizer["DateOfBirthUpdatedSuccess"]) : BadRequest(_localizer["DateOfBirthUpdateError"]);
    }

    // Cập nhật thông tin người dùng chung
    [HttpPost("general-update")]
    public async Task<IActionResult> GeneralUpdate([FromBody] GeneralUpdateRequest request)
    {
        var result = await _userService.GeneralUpdateAsync(request);
        return result ? Ok(_localizer["UserInfoUpdatedSuccess"]) : BadRequest(_localizer["UserInfoUpdateError"]);
    }

    
}
