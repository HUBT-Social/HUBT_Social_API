using System.Globalization;
using System.Net;
using HUBT_Social_API.Core.Configurations;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Src.Core.Configurations;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;

namespace HUBT_Social_API;

    public class Program
    {
        
        private static void InitConfigures(WebApplicationBuilder builder)
        {
            // Cấu hình Swagger
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenService();

            // Cấu hình localization, JWT, Identity, MongoDB, SignalR, Cloudinary và SMTP
            builder.Services.ConfigureLocalization();
            
            builder.Services.ConfigureIdentity(builder.Configuration);
            builder.Services.ConfigureSignalR();
            builder.Services.ConfigureCloudinary(builder.Configuration);
            builder.Services.ConfigureBackblazeB2(builder.Configuration);
            builder.Services.ConfigureSMPT(builder.Configuration);
            //builder.Services.FirebaseService(builder.Configuration);
            builder.Services.AddConfigureationService(builder.Configuration);


            builder.Services.AddChatMongoCollections(builder.Configuration);
            builder.Services.AddAuthMongoCollections(builder.Configuration);
            builder.Services.ConfigureJwt(builder.Configuration);
        }

        private static void InitServices(WebApplicationBuilder builder) 
        {
            // Đăng ký các dịch vụ của hệ thống
            builder.Services.RegisterApplicationServices();

            // Thêm hỗ trợ cho các controller
            builder.Services.AddControllers();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
                        // Gọi hàm cấu hình và đăng ký dịch vụ
            InitConfigures(builder);
            InitServices(builder);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("https://chatuitest.onrender.com")  // Chỉ cho phép origin này
                        .AllowAnyMethod()   // Cho phép bất kỳ phương thức HTTP nào
                        .AllowAnyHeader()   // Cho phép bất kỳ header nào
                        .AllowCredentials(); // Cho phép gửi credentials như cookies, authorization headers
                });
            });

            var app = builder.Build();
            app.UseCors("AllowSpecificOrigin");
            // Cấu hình HTTP request pipeline
            // if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseLocalization();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Định tuyến các controller và SignalR Hub
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }