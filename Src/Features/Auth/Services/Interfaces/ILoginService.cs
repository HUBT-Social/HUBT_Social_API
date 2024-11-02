using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface ILoginService
{
    Task<(SignInResult Result, AUser? User)> LoginAsync(ILoginRequest model);
}