using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    // Cập nhật email
    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email không được để trống.");

        var result = await _userService.UpdateEmailAsync(userResponse.Username,request);
        return result ? Ok("Email đã được cập nhật.") : BadRequest("Có lỗi xảy ra khi cập nhật email.");
    }

    // Cập nhật mật khẩu
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("Mật khẩu hiện tại và mật khẩu mới không được để trống.");

        var result = await _userService.UpdatePasswordAsync(userResponse.Username,request);
        return result ? Ok("Mật khẩu đã được cập nhật.") : BadRequest("Có lỗi xảy ra khi cập nhật mật khẩu.");
    }

    // Cập nhật tên người dùng
    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest("không được để trống.");
        var result = await _userService.UpdateNameAsync(userResponse.Username,request);
        return result ? Ok("Tên người dùng đã được cập nhật.") : BadRequest("Có lỗi xảy ra khi cập nhật tên.");
    }

    // Cập nhật số điện thoại
    [HttpPost("update-phone-number")]
    public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username) || string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest("không được để trống.");

        var result = await _userService.UpdatePhoneNumberAsync(userResponse.Username,request);
        return result ? Ok("Số điện thoại đã được cập nhật.") : BadRequest("Có lỗi xảy ra khi cập nhật số điện thoại.");
    }

    // Cập nhật giới tính
    [HttpPost("update-gender")]
    public async Task<IActionResult> UpdateGender([FromBody] UpdateGenderRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest("không được để trống.");

        var result = await _userService.UpdateGenderAsync(userResponse.Username,request);
        return result ? Ok("Giới tính đã được cập nhật.") : BadRequest("Có lỗi xảy ra khi cập nhật giới tính.");
    }

    // Cập nhật ngày sinh
    [HttpPost("update-date-of-birth")]
    public async Task<IActionResult> UpdateDateOfBirth([FromBody] UpdateDateOfBornRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest("không được để trống.");

        var result = await _userService.UpdateDateOfBirthAsync(userResponse.Username,request);
        return result ? Ok("Ngày sinh đã được cập nhật.") : BadRequest("Có lỗi xảy ra khi cập nhật ngày sinh.");
    }

    // Cập nhật thông tin người dùng chung
    [HttpPost("general-update")]
    public async Task<IActionResult> GeneralUpdate([FromBody] GeneralUpdateRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);
        if (string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest("không được để trống.");

        var result = await _userService.GeneralUpdateAsync(userResponse.Username,request);
        return result
            ? Ok("Thông tin người dùng đã được cập nhật.")
            : BadRequest("Có lỗi xảy ra khi cập nhật thông tin.");
    }

    // Kiểm tra mật khẩu hiện tại
    [HttpPost("check-password")]
    public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest("Tên người dùng và mật khẩu hiện tại không được để trống.");

        var result = await _userService.VerifyCurrentPasswordAsync(userResponse.Username,request);
        return result ? Ok("Mật khẩu đúng.") : BadRequest("Mật khẩu không đúng.");
    }

    // Gửi mã OTP
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Tên người dùng không được để trống.");

        var user = await _userService.FindUserByUserNameAsync(request.Username);

        if (user == null) return BadRequest("Yêu cầu không hợp lệ.");
#pragma warning disable CS8604 // Possible null reference argument.
        var code = await _emailService.CreatePostcodeAsync(user.Email);
#pragma warning restore CS8604 // Possible null reference argument.

        var result = await _emailService.SendEmailAsync(
            new EmailRequest
            {
                Code = code.Code,
                Subject = "Validate Email Code",
                ToEmail = user.Email
            });
        return result ? Ok("Mã OTP đã được gửi.") : BadRequest("Có lỗi xảy ra khi gửi mã OTP.");
    }

    // Xác thực mã OTP
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ValidatePostcodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Postcode))
            return BadRequest("Tên người dùng và mã OTP không được để trống.");

        var result = await _emailService.ValidatePostcodeAsync(request);
        return result is not null ? Ok("Mã OTP xác thực thành công.") : BadRequest("Mã OTP không hợp lệ.");
    }
}