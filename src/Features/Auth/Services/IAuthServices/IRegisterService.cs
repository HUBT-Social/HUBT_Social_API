using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.src.Features.Auth.Services.IAuthServices
{
    public interface IRegisterService
    {
        Task<(IdentityResult,AUser)> RegisterAsync(RegisterRequest model);
    }
}