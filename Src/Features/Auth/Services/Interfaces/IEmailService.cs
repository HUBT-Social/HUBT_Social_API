using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest emailRequest);
    Task<Postcode?> CreatePostcodeSignUpAsync(string userAgent, string receiver, string ipAddress);
    Task<Postcode?> CreatePostcodeSignInAsync(string userAgent, string receiver, string ipAddress);
    Task<Postcode?> CreatePostcodeForgetPasswordAsync(string userAgent, string receiver, string ipAddress);
    Task<AUser?> ValidatePostcodeAsync(ValidatePostcodeRequest postcode);
    Task<Postcode?> GetCurrentPostCode(string userAgent , string ipAddress);
    bool MaskEmail(string email, out string maskEmail);
    bool IsValidEmail(string email);
}