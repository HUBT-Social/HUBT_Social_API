using Amazon.Runtime.Internal;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace HUBT_Social_API.Src.Core.Helpers
{
    public static class PostcodeHelper
    {
        public static async Task<IActionResult> ResendPostcode(
            HttpContext context,
            HttpRequest request,
            Func<string,string,string, Task<Postcode>> sendPostcodeFuc,
            IEmailService _emailService,
            string type)
        {
            string userAgent = request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(context);

            if (ipAddress == null) return new BadRequestObjectResult(LocalValue.Get(KeyStore.InvalidCredentials));

            try
            {
                Postcode? existingPostcode = await _emailService.GetCurrentPostCode(userAgent, ipAddress);

                if (existingPostcode == null || !existingPostcode.PostcodeType.Equals(type))
                    return new BadRequestObjectResult(LocalValue.Get(KeyStore.UserNotFound));

                Postcode? newPostcode = await sendPostcodeFuc(userAgent, existingPostcode.Email, ipAddress);

                if (newPostcode == null)
                    return new BadRequestObjectResult(LocalValue.Get(KeyStore.InvalidCredentials));

                Postcode? code = await sendPostcodeFuc(userAgent, existingPostcode.Email, ipAddress);
                if (code == null) return new BadRequestObjectResult(LocalValue.Get(KeyStore.InvalidCredentials));

                bool result = await _emailService.SendEmailAsync(new EmailRequest
                {
                    Code = code.Code,
                    Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                    ToEmail = existingPostcode.Email
                });
                return result ? new OkObjectResult(LocalValue.Get(KeyStore.OtpSent)) : new BadRequestObjectResult(LocalValue.Get(KeyStore.OtpSendError));
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(
                    LocalValue.Get(KeyStore.UnableToSendOTP)
                );
            }

        }
    }
}
