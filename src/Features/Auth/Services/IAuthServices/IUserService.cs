using HUBT_Social_API.src.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Authentication.Models;

namespace HUBT_Social_API.src.Features.Auth.Services.IAuthServices
{
    public interface IUserService
    {
        Task<AUser> FindUserByUserNameAsync(string userName);
        Task<bool> PromoteUserAccountAsync(string userName, string roleName);
        Task<bool> ChangeLanguage(ChangeLanguageRequest changeLanguageRequest);

    }
}