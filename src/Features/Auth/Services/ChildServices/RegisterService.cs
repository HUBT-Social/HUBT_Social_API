using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AspNetCore.Identity.MongoDbCore.Models;

using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;

namespace HUBT_Social_API.src.Features.Auth.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<AUser> _userManager;
        private readonly RoleManager<ARole> _roleManager;

        public RegisterService(UserManager<AUser> userManager, RoleManager<ARole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(IdentityResult, AUser)> RegisterAsync(RegisterRequest model)
        {
            // Kiểm tra null cho model
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model không thể null.");
            }
            AUser? accountAlreadyExists = await _userManager.FindByNameAsync(model.StudentCode);

            if (accountAlreadyExists is not null)
            {
                throw new Exception("Tài khoản đã được đăng ký.");
            }

            AUser user = new AUser
            {
                UserName = model.StudentCode,
                Email = model.Email,
                FullName = model.FullName
            };

       
            IdentityResult? result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                const string defaultRole = "USER";

             
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new ARole {Name = defaultRole }); // Sử dụng MongoIdentityRole<Guid>
                }

              
                await _userManager.AddToRoleAsync(user, defaultRole);

                
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, defaultRole));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                await _userManager.AddClaimAsync(user, new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
                await _userManager.AddClaimAsync(user, new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
            }
            else
            {
                
            }

            return (result,user);
        }
    }
}
