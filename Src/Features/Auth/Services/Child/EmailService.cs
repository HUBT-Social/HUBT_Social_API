using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Src.Core.HttpContent;
using HUBT_Social_API.Src.Core.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MongoDB.Driver;
using System.ComponentModel;
using System.IO;

using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class EmailService : IEmailService
{
    private readonly SMPTSetting _emailSetting;
    private readonly IMongoCollection<Postcode> _postcode;
    private readonly UserManager<AUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public EmailService(IOptions<SMPTSetting> setting, IMongoCollection<Postcode> postcode,
        UserManager<AUser> userManager,IWebHostEnvironment env)
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
        var smtpHost =_emailSetting.Host;
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
    public async Task<Postcode?> CreatePostcodeByTypeAsync(string userAgent, string receiver, string ipAdress, string type)
    {
        var code = GenerateOtp();

        Postcode? postcode = await _postcode.Find(
            pc => pc.IPAddress == ipAdress && pc.UserAgent == userAgent
            ).FirstOrDefaultAsync();

        if (postcode is not null)
        {
            var updatePostcode = Builders<Postcode>.Update.Set(pc => pc.Code, code)
                .Set(pc => pc.ExpireTime, DateTime.UtcNow)
                .Set(pc => pc.Email, receiver)
                .Set(pc => pc.PostcodeType,type);
            postcode.Code = code;
            await _postcode.UpdateOneAsync(
                pc => pc.IPAddress == ipAdress && pc.UserAgent == userAgent
                , updatePostcode);

            return postcode;
        };
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


    public async Task<AUser?> ValidatePostcodeAsync(ValidatePostcodeRequest postcodeRequest)
    {
        AUser? user = await _userManager.FindByEmailAsync(postcodeRequest.Email);
        if (user == null) return null;

        Postcode userPostcode = await _postcode
            .Find(ps => ps.Code == postcodeRequest.Postcode && ps.Email == postcodeRequest.Email && ps.UserAgent == postcodeRequest.UserAgent)
            .FirstOrDefaultAsync();

        if (userPostcode == null) return null;

        user.EmailConfirmed = true;
        IdentityResult result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded) return null;

        return user;
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
        }catch
        {
            emailHtmlContent = LocalValue.Get(KeyStore.EmailTemplate);

            emailHtmlContent = emailHtmlContent
            .Replace("{{RecipientName}}", emailRequest.FullName.Length != 0 ? emailRequest.FullName : emailRequest.ToEmail)
            .Replace("{{content-top0}}",LocalValue.Get(KeyStore.EmailContentOTP0))
            .Replace("{{content-top1}}",LocalValue.Get(KeyStore.EmailContentOTP1))
            .Replace("{{content-top2}}",LocalValue.Get(KeyStore.EmailContentOTP2))
            .Replace("{{content-bottom2}}",LocalValue.Get(KeyStore.EmailContentOTP4))
            .Replace("{{content-bottom3}}",LocalValue.Get(KeyStore.EmailContentOTP5))
            .Replace("{{content-bottom4}}",LocalValue.Get(KeyStore.EmailContentOTP6))
            .Replace("{{footer1}}",LocalValue.Get(KeyStore.EmailContentFooter1))
            .Replace("{{footer2}}",LocalValue.Get(KeyStore.EmailContentFooter2))
            .Replace("{{footer3}}",LocalValue.Get(KeyStore.EmailContentFooter3))
            .Replace("{{footer4}}",LocalValue.Get(KeyStore.EmailContentFooter4));
        }
        
        
        // Thay thế thông tin trong template
        emailHtmlContent = emailHtmlContent
            .Replace("{{name}}", emailRequest.FullName.Length != 0 ? emailRequest.FullName : emailRequest.ToEmail)
            .Replace("{{device}}",emailRequest.Device)
            .Replace("{{location}}",emailRequest.Location)
            .Replace("{{time}}",emailRequest.DateTime)
            .Replace("{{text0}}",LocalValue.Get(KeyStore.Email2Text0))
            .Replace("{{text1}}",LocalValue.Get(KeyStore.Email2Text1))
            .Replace("{{text2}}",LocalValue.Get(KeyStore.Email2Text2))
            .Replace("{{text3}}",LocalValue.Get(KeyStore.Email2Text3))
            .Replace("{{text4}}",LocalValue.Get(KeyStore.Email2Text4))
            .Replace("{{text5}}",LocalValue.Get(KeyStore.Email2Text5))
            .Replace("{{text6}}",LocalValue.Get(KeyStore.Email2Text6))
            .Replace("{{text7}}",LocalValue.Get(KeyStore.Email2Text7))
            .Replace("{{text8}}",LocalValue.Get(KeyStore.Email2Text8))
            .Replace("{{text9}}",LocalValue.Get(KeyStore.Email2Text9))
            .Replace("{{text10}}",LocalValue.Get(KeyStore.Email2Text10))
            .Replace("{{text11}}",LocalValue.Get(KeyStore.Email2Text11))
            .Replace("{{text12}}",LocalValue.Get(KeyStore.Email2Text12))
            .Replace("{{text13}}",LocalValue.Get(KeyStore.Email2Text13));
        
        for (int i = 0; i < 6; i++)
        {
            string placeholder = $"{{{{value-{i}}}}}";

            string value = emailRequest.Code[i].ToString();
            emailHtmlContent = emailHtmlContent.Replace(placeholder,value);
        }

        // Tạo body của email
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailHtmlContent,
            TextBody = $"Hi {emailRequest.ToEmail},\n\nYour OTP is {string.Join("", emailRequest.Code)}.\n\nBest Regards,\nHUBT Social Team"
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
            Sender = MailboxAddress.Parse(_emailSetting.Email),
            Subject = emailRequest.Subject
        };
        email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
        var htmlContent = SendEmailHttpContent.GetSendPostcodeContent(emailRequest.Code);

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlContent,  
            TextBody = $"Your code is: {emailRequest.Code}. Thank you for using our service!"
        };
        email.Body = bodyBuilder.ToMessageBody();
        return email;
    }





    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }



    public async Task<Postcode?> GetCurrentPostCode(string userAgent , string ipAddress)
    {
        Postcode postcode = await _postcode
            .Find(ps => ps.UserAgent == userAgent && ps.IPAddress == ipAddress)
            .FirstOrDefaultAsync();

        if (postcode != null) return postcode;

        return null;
    }

    public bool MaskEmail(string email,out string maskEmail)
    {
        string[] emailParts = email.Split('@');
        maskEmail = null;

        if (emailParts.Length == 2)
        {
            string username = emailParts[0];
            string domain = emailParts[1];

            string maskedUsername = username.Substring(0,3) + new string('*', Math.Max(0, username.Length));

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
    
}