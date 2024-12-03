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

    public async Task<bool> UploadMessageAsync(MessageRequest chatRequest)
    {
        // Tạo một tin nhắn mới
        MessageModel newMessage = new()
        {
            UserId = chatRequest.UserId,
            Content = new List<string>(),
            Type = HUBTSOCIAL.Src.Features.Chat.Models.Type.Message
        };

        newMessage.Content.Add(chatRequest.Message);



        // Cập nhật vào MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.Messages, newMessage);

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

        return result.ModifiedCount > 0;
    }
    public async Task<bool> UploadFileAsync(FileRequest chatRequest)
    {
        // Tạo một tin nhắn mới
        MessageModel newMessage = new()
        {
            UserId = chatRequest.UserId,
            Content = new List<string>(),
            Type = HUBTSOCIAL.Src.Features.Chat.Models.Type.Message
        };


        // Xử lý danh sách file tải lên
        if (chatRequest.Files != null)
        {
             foreach (var file in chatRequest.Files)
             {
                 var fileUrl = await UploadToStorageAsync(file);

                 newMessage.Content.Add(fileUrl);
             }
         }

        // Cập nhật vào MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.Messages, newMessage);

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

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