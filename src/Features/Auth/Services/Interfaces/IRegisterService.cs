using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IRegisterService
{
    Task<(IdentityResult Result, AUser? User, string? Error)> RegisterAsync(RegisterRequest model);
}