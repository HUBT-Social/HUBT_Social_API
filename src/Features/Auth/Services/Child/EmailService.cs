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
using System;

namespace HUBT_Social_API.Features.Auth.Services.Child
{
    public class EmailService : IEmailService
    {
        private readonly SMPTSetting _emailSetting;
        private readonly IMongoCollection<Postcode> _postcode;
        private readonly UserManager<AUser> _userManager;

        public EmailService(IOptions<SMPTSetting> setting, IMongoCollection<Postcode> postcode, UserManager<AUser> userManager)
        {
            _emailSetting = setting.Value;
            _postcode = postcode;
            _userManager = userManager;
        }

        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var email = CreateEmailMessage(emailRequest);
                using var smtpClient = new SmtpClient();

                // Lấy thông tin từ môi trường hoặc cài đặt mặc định
                var smtpHost = GetEnvironmentVariable("SMTP_HOST") ?? _emailSetting.Host;
                var smtpPort = int.Parse(GetEnvironmentVariable("SMTP_PORT") ?? _emailSetting.Port);
                var smtpEmail = GetEnvironmentVariable("SMTP_USERNAME") ?? _emailSetting.Email;
                var smtpPassword = GetEnvironmentVariable("SMTP_PASSWORD") ?? _emailSetting.Password;

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

        public async Task<Postcode> CreatePostcodeAsync(string receiver)
        {
            var code = GenerateOtp();
            var postcode = new Postcode
            {
                Code = code,
                Email = receiver,
                ExpireTime = DateTime.UtcNow 
            };

            await _postcode.InsertOneAsync(postcode);
            return postcode;
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<AUser?> ValidatePostcodeAsync(ValidatePostcodeRequest postcodeRequest)
        {
            var user = await _userManager.FindByEmailAsync(postcodeRequest.Email);
            if (user == null) return null;

            var userPostcode = await _postcode.Find(ps => ps.Code == postcodeRequest.Postcode && ps.Email == postcodeRequest.Email)
                .FirstOrDefaultAsync();

            // Kiểm tra xem mã OTP đã hết hạn hay chưa
            if (userPostcode == null || userPostcode.ExpireTime < DateTime.UtcNow) return null;

            return user;
        }

        private string? GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

    }
}
