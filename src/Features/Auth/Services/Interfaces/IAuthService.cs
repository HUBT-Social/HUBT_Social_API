using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<(IdentityResult, AUser?, string?)> RegisterAsync(RegisterRequest model);
    Task<(SignInResult, AUser?, string?)> LoginAsync(ILoginRequest model);

    Task<AUser> VerifyCodeAsync(VLpostcodeRequest request);

}