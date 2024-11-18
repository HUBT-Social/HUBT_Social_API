using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IRegisterService
{
    Task<(IdentityResult Result, AUser? user)> RegisterAsync(RegisterRequest model);

    Task<bool> CheckUserAccountExit(RegisterRequest model);

    Task<bool> AddToTempUser(RegisterRequest model);

    Task<TempUserRegister?> GetTempUser(string Email);
}