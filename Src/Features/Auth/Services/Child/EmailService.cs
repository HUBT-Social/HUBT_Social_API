using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class EmailService : IEmailService
{
    private readonly SMPTSetting _emailSetting;
    private readonly IMongoCollection<Postcode> _postcode;
    private readonly UserManager<AUser> _userManager;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpEmail;
    private readonly string _smtpPassword;
    

    public EmailService(IOptions<SMPTSetting> setting, IMongoCollection<Postcode> postcode,
        UserManager<AUser> userManager)
    {
        _emailSetting = setting.Value;
        _postcode = postcode;
        _userManager = userManager;
        // Lấy thông tin từ môi trường hoặc cài đặt mặc định
        _smtpHost = GetEnvironmentVariable("SMTP_HOST") ?? _emailSetting.Host;
        _smtpPort = int.Parse(GetEnvironmentVariable("SMTP_PORT") ?? _emailSetting.Port);
        _smtpEmail = GetEnvironmentVariable("SMTP_USERNAME") ?? _emailSetting.Email;
        _smtpPassword = GetEnvironmentVariable("SMTP_PASSWORD") ?? _emailSetting.Password;
    }

    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        try
        {
            var email = CreateEmailMessage(emailRequest);
            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_smtpEmail, _smtpPassword);
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

    public async Task<Postcode?> CreatePostcodeAsync(string userAgent,string receiver)
    {
        var code = GenerateOtp();

        Postcode? postcode = await _postcode.Find(
            pc => pc.Email == receiver && pc.UserAgent == userAgent
            ).FirstOrDefaultAsync();

        if (postcode is not null)
        {
            var updatePostcode = Builders<Postcode>.Update.Set(pc => pc.Code , code)
                .Set(pc=> pc.ExpireTime , DateTime.UtcNow);
            postcode.Code = code;
            await _postcode.UpdateOneAsync(
                pc => pc.Email == receiver && pc.UserAgent == userAgent
                ,updatePostcode);

            return postcode;
        }; 

        Postcode newPostcode = new()
        {
            UserAgent = userAgent,
            Code = code,
            Email = receiver,
            ExpireTime = DateTime.UtcNow
        };

        await _postcode.InsertOneAsync(newPostcode);
        return newPostcode;
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
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_emailSetting.Email),
            Subject = emailRequest.Subject
        };
        email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
        email.Body = new TextPart(TextFormat.Plain)
        {
            Text = emailRequest.Code
        };
        return email;
    }

    private string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    private string? GetEnvironmentVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }

    public async Task<string?> GetValidateEmail(string userAgent)
    {
        Postcode postcode = await _postcode
            .Find(ps => ps.UserAgent == userAgent)
            .FirstOrDefaultAsync();

        if (postcode == null) return null;

        return postcode.Email;
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
}