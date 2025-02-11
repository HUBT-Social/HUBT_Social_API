using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Src.Core.Helpers;

public static class PostcodeHelper
{
    public static async Task<IActionResult> ResendPostcode(
        HttpContext context,
        HttpRequest request,
        Func<string, string, string, Task<Postcode>> sendPostcodeFuc,
        IEmailService _emailService,
        string type)
    {
        var userAgent = request.Headers.UserAgent.ToString();
        var ipAddress = ServerHelper.GetIPAddress(context);

        if (ipAddress == null) return new BadRequestObjectResult(LocalValue.Get(KeyStore.InvalidCredentials));

        try
        {
            var existingPostcode = await _emailService.GetCurrentPostCode(userAgent, ipAddress);

            if (existingPostcode == null || !existingPostcode.PostcodeType.Equals(type))
                return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));

            var newPostcode = await sendPostcodeFuc(userAgent, existingPostcode.Email, ipAddress);

            if (newPostcode == null)
                return new BadRequestObjectResult(LocalValue.Get(KeyStore.InvalidCredentials));

            var code = await sendPostcodeFuc(userAgent, existingPostcode.Email, ipAddress);
            if (code == null) return new BadRequestObjectResult(LocalValue.Get(KeyStore.InvalidCredentials));

            var result = await _emailService.SendEmailAsync(new EmailRequest
            {
                Code = code.Code,
                Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                ToEmail = existingPostcode.Email,
                Device = userAgent,
                Location = await ServerHelper.GetLocationFromIpAsync(ipAddress),
                DateTime = ServerHelper.ConvertToCustomString(DateTime.UtcNow)
            });
            return result
                ? new OkObjectResult(LocalValue.Get(KeyStore.OtpSent))
                : new BadRequestObjectResult(LocalValue.Get(KeyStore.OtpSendError));
        }
        catch (Exception)
        {
            return new BadRequestObjectResult(
                LocalValue.Get(KeyStore.UnableToSendOTP)
            );
        }
    }
}