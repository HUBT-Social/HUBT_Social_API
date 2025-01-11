using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;
using HUBT_Social_API.Features.Chat.DTOs;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HUBT_Social_API.Features.Chat.ChatHubs;
using Microsoft.AspNetCore.SignalR;

using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Core.Service.Upload;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public class UploadChatServices : IUploadChatServices
{
    private readonly IMessageUploadService _messageUploadService;
    private readonly IMediaUploadService _mediaUploadService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IHubContext<ChatHub> _hubContext;
    public UploadChatServices
    (
        IMessageUploadService messageUploadService,
        IMediaUploadService mediaUploadService,
        IFileUploadService fileUploadService,
        IHubContext<ChatHub> hubContext
    )
    {
        _messageUploadService = messageUploadService;
        _mediaUploadService = mediaUploadService;
        _fileUploadService = fileUploadService;
        _hubContext = hubContext;
        
    }
    public async Task<bool> SendMessageAsync(MessageRequest chatRequest,string eventName) 
        => await _messageUploadService.UploadMessageAsync(chatRequest,_hubContext,eventName);
         
    public async Task<bool> SendMediaAsync(MediaRequest chatRequest,string eventName)
        => await _mediaUploadService.UploadMediaAsync(chatRequest,_hubContext,eventName);

    public Task<bool> SendFileAsync(IFormFile file)
        => _fileUploadService.UploadFileAsync(file);


}






































