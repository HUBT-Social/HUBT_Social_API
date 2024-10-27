using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IUserService
{
    Task<AUser> FindUserByUserNameAsync(string userName);
    Task<bool> PromoteUserAccountAsync(string userName, string roleName);

}