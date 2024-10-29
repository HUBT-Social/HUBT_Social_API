using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest emailRequest);
    Task<Postcode> CreatePostcodeAsync(string reciver);
    Task<AUser> ValidatePostcodeAsync(ValidatePostcodeRequest postcode);
}