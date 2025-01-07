using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

namespace HUBT_Social_API.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IEmailService _emailService;
    private readonly ILoginService _loginService;
    private readonly IRegisterService _registerService;
    private readonly IUserService _userService;
    private readonly IMongoCollection<UserFCMToken> _userFCMTokens;

    public AuthService(IRegisterService registerService, ILoginService loginService, IUserService userService,
        IEmailService emailService , IMongoCollection<UserFCMToken> userFCMTokens)
    {
        _registerService = registerService;
        _loginService = loginService;
        _userService = userService;
        _emailService = emailService;
        _userFCMTokens = userFCMTokens;
    }


    public async Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model)
    {
        return await _loginService.LoginAsync(model);
    }

    public async Task<AUser?> VerifyCodeAsync(ValidatePostcodeRequest request)
    {
        return await _emailService.ValidatePostcodeAsync(request);
    }

    public async Task<(IdentityResult, AUser?)> RegisterAsync(RegisterRequest model)
    {
        return await _registerService.RegisterAsync(model);
    }

    public async Task<TempUserRegister?> GetTempUser(string email)
    {
        return await _registerService.GetTempUser(email);
    }

    public async Task<bool> StoreFcmTokenAsync(StoreFCMRequest request)
    {
        try
        {
            UserFCMToken fcmToken = await _userFCMTokens.Find(
                fcm => fcm.DeviceId == request.DeviceID && fcm.UserId == request.UserID )
                .FirstOrDefaultAsync();
            if (fcmToken == null)
            {
                UserFCMToken newFcmToken = new()
                {
                    FcmToken = request.FcmToken,
                    UserId = request.UserID,
                    DeviceId = request.DeviceID,
                };
                await _userFCMTokens.InsertOneAsync(newFcmToken);
                
            }
            var update = Builders<UserFCMToken>.Update
                .Set(t => t.FcmToken, request.FcmToken);
            await _userFCMTokens.UpdateOneAsync(
                fcm => fcm.DeviceId == request.DeviceID && fcm.UserId == request.UserID
            , update);

            return true;
            
        }
        catch (Exception)
        {
            return false;
        }
        
    }
}