using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
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

    public async Task<(IdentityResult, AUser?,string?)> RegisterAsync(RegisterRequest model)
    {
        return await _registerService.RegisterAsync(model);
    }

    public async Task<(SignInResult, AUser?, string?)> LoginAsync(ILoginRequest model)
    {
        return await _loginService.LoginAsync(model);
    }

    public async Task<AUser> VerifyCodeAsync(VLpostcodeRequest request)
    {
        return await _emailService.ValidatePostcode(request);
    }
}