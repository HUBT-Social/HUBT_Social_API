using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<(IdentityResult, AUser?)> RegisterAsync(RegisterRequest model);
    Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model);

    Task<AUser?> VerifyCodeAsync(ValidatePostcodeRequest request);

    Task<TempUserRegister?> GetTempUser(string email);

}