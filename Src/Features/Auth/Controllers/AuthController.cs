using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public partial class AuthController : ControllerBase
{
    protected readonly IAuthService _authService;
    protected readonly IEmailService _emailService;
    protected readonly IRegisterService _registerService;
    protected readonly ITokenService _tokenService;
    protected readonly IUserService? _userService;

    public AuthController(IAuthService authService, ITokenService tokenService, IEmailService emailService,
        IRegisterService registerService, IUserService userService)
    {
        _authService = authService;
        _tokenService = tokenService;
        _emailService = emailService;
        _registerService = registerService;
        _userService = userService;
    }
}