using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;
using HUBT_Social_API.Features.Chat.DTOs;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public class UploadChatServices : IUploadServices
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly Cloudinary _cloudinary;

    public UploadChatServices(IMongoCollection<ChatRoomModel> chatRooms, Cloudinary cloudinary)
    {
        _chatRooms = chatRooms;
        _cloudinary = cloudinary;
    }

    public async Task<bool> UploadChatAsync(ChatRequest chatRequest)
    {
        // Tạo một tin nhắn mới
        MessageModel newMessage = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = chatRequest.UserId,
            Timestamp = DateTime.UtcNow,
            Content = new ContentModel
            {
                Message = chatRequest.Message
            }
        };

        // Xử lý danh sách file tải lên
        if (chatRequest.Media != null)
        {
            foreach (var file in chatRequest.Media)
            {
                var fileUrl = await UploadToStorageAsync(file);
                MediaType mediaType = DetermineMediaType(file.ContentType);

                newMessage.Content.Media.Add(new Media
                {
                    Url = fileUrl,
                    FileType = file.ContentType,
                    Size = file.Length,
                    MediaType = mediaType
                });
            }
        }

        // Cập nhật vào MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.Messages, newMessage);

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

        return result.ModifiedCount > 0;
    }

    private MediaType DetermineMediaType(string contentType)
    {
        if (contentType.StartsWith("image")) return MediaType.Image;
        if (contentType.StartsWith("video")) return MediaType.Video;
        if (contentType.StartsWith("audio")) return MediaType.Audio;
        return MediaType.File; // Mặc định là file
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