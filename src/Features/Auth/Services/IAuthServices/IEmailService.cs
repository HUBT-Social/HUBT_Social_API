using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Authentication.Models;


namespace HUBT_Social_API.src.Features.Auth.Services.IAuthServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest emailRequest);
        Task<Postcode> CreatePostcode(string reciver);
        Task<AUser> ValidatePostcode(VLpostcodeRequest postcode);
    }
}
