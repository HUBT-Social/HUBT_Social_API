using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;
using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public class UpdateChatServices 
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly Cloudinary _cloudinary;

    public UpdateChatServices(IMongoCollection<ChatRoomModel> chatRooms, Cloudinary cloudinary)
    {
        _chatRooms = chatRooms;
        _cloudinary = cloudinary;
    }

    public async Task<bool> UnsendMessageAsync(UnsendMessageRequest chatRequest)
    {
        return false;
       
    }




}