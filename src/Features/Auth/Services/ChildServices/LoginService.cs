using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.src.Features.Auth.Services
{
    public class LoginService : ILoginService
    {
        private readonly SignInManager<AUser> _signInManager;
        private readonly UserManager<AUser> _userManager;

        public LoginService(SignInManager<AUser> signInManager, UserManager<AUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<(SignInResult,AUser?)> LoginAsync(ILoginRequest model)
        {
            var user = await FindUserByIdentifierAsync(model);
            if (user == null) return (new SignInResult(),null);

            SignInResult? result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);           
            return result.Succeeded 
                ? (result,await _userManager.FindByNameAsync(model.Identifier)) 
                : (result,null);
        }


        private async Task<AUser?> FindUserByIdentifierAsync(ILoginRequest identifier)
        {
            AUser? user = null;


            if (new EmailAddressAttribute().IsValid(identifier.Identifier))
            {
                user = await _userManager.FindByEmailAsync(identifier.Identifier);
            }

            else
            {
                user = await _userManager.FindByNameAsync(identifier.Identifier);
                if (!await _userManager.CheckPasswordAsync(user, identifier.Password))
                {
                    return null;
                }
            }

            return user;
        }
    }
}