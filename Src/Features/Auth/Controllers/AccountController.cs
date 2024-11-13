using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public partial class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IRegisterService _registerService;
    private readonly ITokenService _tokenService;
    

    public AccountController(IAuthService authService,ITokenService tokenService, IEmailService emailService, IRegisterService registerService)
    {
        _authService = authService;
        _tokenService = tokenService;
        _emailService = emailService;
        _registerService = registerService;
    }

}
