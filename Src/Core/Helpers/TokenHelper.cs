using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace HUBT_Social_API.Src.Core.Helpers
{
    public static class TokenHelper
    {
        public static string? ExtractTokenFromHeader(HttpRequest request)
        {
            var token = request.Headers.Authorization.ToString();
            return !string.IsNullOrEmpty(token) ? token.Replace("Bearer ", "") : null;
        }

        public static async Task<UserResponse> GetUserResponseFromToken(HttpRequest request, ITokenService tokenService)
        {
            var token = ExtractTokenFromHeader(request);
            if (string.IsNullOrEmpty(token))
                return new UserResponse { Success = false };

            return await tokenService.GetCurrentUser(token);
        }
        public static async Task<List<string>> ConvertRolesIdtoRolesName(List<Guid> guids, RoleManager<ARole> roleManager)
        {
            var roleNames = new List<string>();

            foreach (var id in guids)
            {
                var role = await roleManager.FindByIdAsync(id.ToString());
                if (role != null)
                {
                    roleNames.Add(role.Name);
                }
            }

            return roleNames;
        }
    }
}
