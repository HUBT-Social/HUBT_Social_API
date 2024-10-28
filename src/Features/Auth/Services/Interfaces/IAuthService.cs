using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<(IdentityResult, string?)> RegisterAsync(RegisterRequest model);
    Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model);

    Task<TempUserRegister> VerifyCodeAsync(ValidatePostcodeRequest request);

}