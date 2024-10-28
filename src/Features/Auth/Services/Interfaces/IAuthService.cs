using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<(IdentityResult, string?)> RegisterAsync(RegisterRequest model);
    Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model);

    Task<AUser> VerifyCodeAsync(ValidatePostcodeRequest request);

}