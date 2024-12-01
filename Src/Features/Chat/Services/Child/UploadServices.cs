using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;
using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public class UploadServices : IUploadServices
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly Cloudinary _cloudinary;

    public UploadServices(IMongoCollection<ChatRoomModel> chatRooms, Cloudinary cloudinary)
    {
        _chatRooms = chatRooms;
        _cloudinary = cloudinary;
    }

    public async Task<bool> UploadChatAsync(ChatRequest chatRequest)
    {
        MessageModel newMessage = new()
        {
            UserId = chatRequest.UserId,
            Content = new ContentModel
            {
                Message = chatRequest.Message,
                Images = null,
                Files = null

            },
            Timestamp = DateTime.UtcNow
        };
        

        if(chatRequest.Images != null)
        {
            List<string> ImageUrls = new();
            foreach (var image in chatRequest.Images)
            {
                var ImgUrl = await UploadToStorageAsync(image);
                ImageUrls.Add(ImgUrl);
            }
            newMessage.Content.Images = ImageUrls;

        }
        if(chatRequest.File != null)
        {
            var fileUrl = await UploadToStorageAsync(chatRequest.File);
            newMessage.Content.Files = fileUrl;
        }
        
        // Tạo cập nhật MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.Messages, newMessage);

        // Thực hiện cập nhật
        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

        // Trả về kết quả
        return result.ModifiedCount > 0;
    }

    public async Task<string> UploadToStorageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream)
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.Url.ToString();
    }

}