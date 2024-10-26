

using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;


namespace HUBTSOCIAL.Src.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthService(IRegisterService registerService, ILoginService loginService, IUserService userService, IEmailService emailService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _userService = userService;
            _emailService = emailService;
        }

        public async Task<(IdentityResult, AUser)> RegisterAsync(RegisterRequest model)
        {
            return await _registerService.RegisterAsync(model);
        }

        public async Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model)
        {
            return await _loginService.LoginAsync(model);
        }


        public async Task<bool> ChangeLanguage(ChangeLanguageRequest changeLanguageRequest)
        {
            return await _userService.ChangeLanguage(changeLanguageRequest);
        }


        public async Task<AUser> VerifyCodeAsync(VLpostcodeRequest request)
        {
            return await _emailService.ValidatePostcode(request);
        }

        
    }
}
