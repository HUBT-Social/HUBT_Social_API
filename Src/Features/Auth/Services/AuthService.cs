using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IEmailService _emailService;
    private readonly ILoginService _loginService;
    private readonly IRegisterService _registerService;
    private readonly IUserService _userService;

    public AuthService(IRegisterService registerService, ILoginService loginService, IUserService userService,
        IEmailService emailService)
    {
        _registerService = registerService;
        _loginService = loginService;
        _userService = userService;
        _emailService = emailService;
    }


    public async Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model)
    {
        return await _loginService.LoginAsync(model);
    }

    public async Task<AUser> VerifyCodeAsync(ValidatePostcodeRequest request)
    {
        return await _emailService.ValidatePostcodeAsync(request);
    }

    public async Task<(IdentityResult, AUser?)> RegisterAsync(RegisterRequest model)
    {
        return await _registerService.RegisterAsync(model);
    }

    public async Task<TempUserRegister> GetTempUser(string email)
    {
        return await _registerService.GetTempUser(email);
    }
}