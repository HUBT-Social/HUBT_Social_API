using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public partial class AuthController : BaseAuthController
{
    public AuthController(IAuthService authService,ITokenService tokenService, IEmailService emailService, IRegisterService registerService,IUserService userService)
    : base(authService, tokenService,emailService,registerService,userService)
    {
    }

}
