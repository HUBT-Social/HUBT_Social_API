using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace HUBT_Social_API.Src.Core.Helpers
{
    public static class UpdateHelper
    {
        public static async Task<IActionResult> HandleUserUpdate<TRequest>(
            string successMessage,
            string errorMessage,
            Func<string, TRequest, Task<bool>> updateFunc,
            TRequest userUpdateRequest,
            HttpRequest request,
            ITokenService tokenService)
        {
            // Lấy thông tin người dùng từ token
            UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(request, tokenService);

            if (string.IsNullOrWhiteSpace(userResponse?.User?.UserName))
            {
                return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));
            }

            // Thực hiện cập nhật
            var result = await updateFunc(userResponse.User.UserName, userUpdateRequest);
            return result
                ? new OkObjectResult(LocalValue.Get(successMessage))
                : new BadRequestObjectResult(LocalValue.Get(errorMessage));
        }
        public static async Task<IActionResult> HandleUserUpdateForgotPassword<TRequest>(
            string successMessage,
            string errorMessage,
            Func<string, TRequest, Task<bool>> updateFunc,
            TRequest userUpdateRequest,
            AUser user)
        {

            if (string.IsNullOrWhiteSpace(user?.UserName))
            {
                return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));
            }


            var result = await updateFunc(user.UserName, userUpdateRequest);
            return result
                ? new OkObjectResult(LocalValue.Get(successMessage))
                : new BadRequestObjectResult(LocalValue.Get(errorMessage));
        }
    }
}
