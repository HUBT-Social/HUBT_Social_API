using System.Net.Mail;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Src.Core.Settings;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MongoDB.Driver;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class EmailService : IEmailService
{
    private readonly SMPTSetting _emailSetting;
    private readonly IWebHostEnvironment _env;
    private readonly IMongoCollection<Postcode> _postcode;
    private readonly UserManager<AUser> _userManager;

    public EmailService(IOptions<SMPTSetting> setting, IMongoCollection<Postcode> postcode,
        UserManager<AUser> userManager, IWebHostEnvironment env)
    {
        _emailSetting = setting.Value;
        _postcode = postcode;
        _userManager = userManager;
        // Lấy thông tin từ môi trường hoặc cài đặt mặc định
        _env = env;
    }

    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        // Lấy thông tin SMTP từ môi trường hoặc cấu hình
        var smtpHost = _emailSetting.Host;
        var smtpPort = int.Parse(_emailSetting.Port);
        var smtpEmail = _emailSetting.Email;
        var smtpPassword = _emailSetting.Password;

        try
        {
            var email = CreateEmailMessage(emailRequest);
            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(smtpEmail, smtpPassword);
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            // Ghi log lỗi ở đây
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }

    public async Task<Postcode?> CreatePostcodeSignUpAsync(string userAgent, string receiver, string ipAddress)
    {
        return await CreatePostcodeByTypeAsync(userAgent, receiver, ipAddress, PostcodeType.SignUp);
    }

    public async Task<Postcode?> CreatePostcodeSignInAsync(string userAgent, string receiver, string ipAddress)
    {
        return await CreatePostcodeByTypeAsync(userAgent, receiver, ipAddress, PostcodeType.SignIn);
    }

    public async Task<Postcode?> CreatePostcodeForgetPasswordAsync(string userAgent, string receiver, string ipAddress)
    {
        return await CreatePostcodeByTypeAsync(userAgent, receiver, ipAddress, PostcodeType.ForgetPassword);
    }


    public async Task<AUser?> ValidatePostcodeAsync(ValidatePostcodeRequest postcodeRequest)
    {
        var user = await _userManager.FindByEmailAsync(postcodeRequest.Email);
        if (user == null) return null;

        var userPostcode = await _postcode
            .Find(ps => ps.Code == postcodeRequest.Postcode && ps.Email == postcodeRequest.Email &&
                        ps.UserAgent == postcodeRequest.UserAgent)
            .FirstOrDefaultAsync();

        if (userPostcode == null) return null;

        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded) return null;

        return user;
    }

    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


    public async Task<Postcode?> GetCurrentPostCode(string userAgent, string ipAddress)
    {
        var postcode = await _postcode
            .Find(ps => ps.UserAgent == userAgent && ps.IPAddress == ipAddress)
            .FirstOrDefaultAsync();

        if (postcode != null) return postcode;

        return null;
    }

    public bool MaskEmail(string email, out string maskEmail)
    {
        string[] emailParts = email.Split('@');
        maskEmail = null;

        if (emailParts.Length == 2)
        {
            var username = emailParts[0];
            var domain = emailParts[1];

            var maskedUsername = username.Substring(0, 3) + new string('*', Math.Max(0, username.Length));

            var domainParts = domain.Split('.');
            string maskedDomain = new('*', domainParts[0].Length);

            maskEmail = $"{maskedUsername}@{maskedDomain}.{domainParts[1]}";
            return true;
        }


        return false;
    }

    public async Task<bool> UpdatePostcode(Postcode postcode)
    {
        try
        {
            var updatePostcode = Builders<Postcode>.Update.Set(pc => pc.Code, postcode.Code)
                .Set(pc => pc.ExpireTime, DateTime.UtcNow)
                .Set(pc => pc.Email, postcode.Email)
                .Set(pc => pc.PostcodeType, postcode.PostcodeType);
            await _postcode.UpdateOneAsync(
                pc => pc.IPAddress == postcode.IPAddress && pc.UserAgent == postcode.UserAgent
                , updatePostcode);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public async Task<Postcode?> CreatePostcodeByTypeAsync(string userAgent, string receiver, string ipAdress,
        string type)
    {
        var code = GenerateOtp();

        var postcode = await _postcode.Find(
            pc => pc.IPAddress == ipAdress && pc.UserAgent == userAgent
        ).FirstOrDefaultAsync();

        if (postcode is not null)
        {
            var updatePostcode = Builders<Postcode>.Update.Set(pc => pc.Code, code)
                .Set(pc => pc.ExpireTime, DateTime.UtcNow)
                .Set(pc => pc.Email, receiver)
                .Set(pc => pc.PostcodeType, type);
            postcode.Code = code;
            await _postcode.UpdateOneAsync(
                pc => pc.IPAddress == ipAdress && pc.UserAgent == userAgent
                , updatePostcode);

            return postcode;
        }

        ;
        try
        {
            Postcode newPostcode = new()
            {
                UserAgent = userAgent,
                IPAddress = ipAdress,
                Code = code,
                Email = receiver,
                PostcodeType = type,
                ExpireTime = DateTime.UtcNow
            };

            await _postcode.InsertOneAsync(newPostcode);
            return newPostcode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return null;
    }

    private MimeMessage CreateEmailMessage(EmailRequest emailRequest)
    {
        var emailMessage = new MimeMessage();
        string emailHtmlContent;
        // Người gửi và người nhận
        emailMessage.From.Add(new MailboxAddress("HUBT Social", _emailSetting.Email));
        emailMessage.To.Add(new MailboxAddress(emailRequest.ToEmail, emailRequest.ToEmail));
        emailMessage.Subject = emailRequest.Subject;
        try
        {
            // Đọc HTML template
            var filePath = Path.Combine(_env.ContentRootPath, "HTML_Template", "OTPVerify.html");
            emailHtmlContent = File.ReadAllText(filePath);
        }
        catch
        {
            emailHtmlContent = LocalValue.Get(KeyStore.EmailTemplate);

            emailHtmlContent = emailHtmlContent
                .Replace("{{RecipientName}}",
                    emailRequest.FullName.Length != 0 ? emailRequest.FullName : emailRequest.ToEmail)
                .Replace("{{content-top0}}", LocalValue.Get(KeyStore.EmailContentOTP0))
                .Replace("{{content-top1}}", LocalValue.Get(KeyStore.EmailContentOTP1))
                .Replace("{{content-top2}}", LocalValue.Get(KeyStore.EmailContentOTP2))
                .Replace("{{content-bottom2}}", LocalValue.Get(KeyStore.EmailContentOTP4))
                .Replace("{{content-bottom3}}", LocalValue.Get(KeyStore.EmailContentOTP5))
                .Replace("{{content-bottom4}}", LocalValue.Get(KeyStore.EmailContentOTP6))
                .Replace("{{footer1}}", LocalValue.Get(KeyStore.EmailContentFooter1))
                .Replace("{{footer2}}", LocalValue.Get(KeyStore.EmailContentFooter2))
                .Replace("{{footer3}}", LocalValue.Get(KeyStore.EmailContentFooter3))
                .Replace("{{footer4}}", LocalValue.Get(KeyStore.EmailContentFooter4));
        }


        // Thay thế thông tin trong template
        emailHtmlContent = emailHtmlContent
            .Replace("{{name}}", emailRequest.FullName.Length != 0 ? emailRequest.FullName : emailRequest.ToEmail)
            .Replace("{{device}}", emailRequest.Device)
            .Replace("{{location}}", emailRequest.Location)
            .Replace("{{time}}", emailRequest.DateTime)
            .Replace("{{text0}}", LocalValue.Get(KeyStore.Email2Text0))
            .Replace("{{text1}}", LocalValue.Get(KeyStore.Email2Text1))
            .Replace("{{text2}}", LocalValue.Get(KeyStore.Email2Text2))
            .Replace("{{text3}}", LocalValue.Get(KeyStore.Email2Text3))
            .Replace("{{text4}}", LocalValue.Get(KeyStore.Email2Text4))
            .Replace("{{text5}}", LocalValue.Get(KeyStore.Email2Text5))
            .Replace("{{text6}}", LocalValue.Get(KeyStore.Email2Text6))
            .Replace("{{text7}}", LocalValue.Get(KeyStore.Email2Text7))
            .Replace("{{text8}}", LocalValue.Get(KeyStore.Email2Text8))
            .Replace("{{text9}}", LocalValue.Get(KeyStore.Email2Text9))
            .Replace("{{text10}}", LocalValue.Get(KeyStore.Email2Text10))
            .Replace("{{text11}}", LocalValue.Get(KeyStore.Email2Text11))
            .Replace("{{text12}}", LocalValue.Get(KeyStore.Email2Text12))
            .Replace("{{text13}}", LocalValue.Get(KeyStore.Email2Text13));

        for (var i = 0; i < 6; i++)
        {
            var placeholder = $"{{{{value-{i}}}}}";

            var value = emailRequest.Code[i].ToString();
            emailHtmlContent = emailHtmlContent.Replace(placeholder, value);
        }

        // Tạo body của email
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailHtmlContent,
            TextBody =
                $"Hi {emailRequest.ToEmail},\n\nYour OTP is {string.Join("", emailRequest.Code)}.\n\nBest Regards,\nHUBT Social Team"
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }
}