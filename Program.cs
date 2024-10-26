<<<<<<< Updated upstream
=======
﻿using HUBT_Social_API.src.Core.Configurations;
using HUBTSOCIAL.Src.Features.Chat.ChatHubs;


>>>>>>> Stashed changes
namespace HUBT_Social_API;

public class Program
{
    private static void Configures(WebApplicationBuilder builder)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        // Cấu hình localization, JWT, Identity, MongoDB, SignalR, Cloudinary và SMTP
        builder.Services.ConfigureLocalization();
        builder.Services.ConfigureJwt(builder.Configuration);
        builder.Services.ConfigureIdentity(builder.Configuration);
        builder.Services.ConfigureSignalR();
        builder.Services.ConfigureCloudinary(builder.Configuration);
        builder.Services.ConfigureSMPT(builder.Configuration);
        builder.Services.FirebaseService(builder.Configuration);
        builder.Services.AddConfigureationService(builder.Configuration);
        builder.Services.ConfigureLocalization(); //Them cau hinh da ngon ngu
        // collection
        builder.Services.AddAuthMongoCollections(builder.Configuration);
        builder.Services.AddAuthMongoCollections(builder.Configuration);
    }

    private static void Services(WebApplicationBuilder builder)
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
        Configures(builder);
        Services(builder);


        var app = builder.Build();

        // Cấu hình HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseLocalization();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}