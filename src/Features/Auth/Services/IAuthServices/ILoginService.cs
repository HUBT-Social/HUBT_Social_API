
using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.src.Features.Auth.Services.IAuthServices
{
    public interface ILoginService
    {
        Task<(SignInResult,AUser?)> LoginAsync(ILoginRequest model);
    }
}