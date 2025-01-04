
using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

public class MediaUploadService : IMediaUploadService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly ChatHub _chatHub;
    public MediaUploadService(ChatHub chatHub,IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatHub = chatHub;
        _chatRooms = chatRooms;
    }
    public async Task<bool> UploadMediaAsync(MediaRequest mediaRequest)
    {
        
        // Lấy ChatRoom từ MongoDB
        FilterDefinition<ChatRoomModel> filterGetChatRoom = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, mediaRequest.GroupId);
        ChatRoomModel chatRoom = await _chatRooms.Find(filterGetChatRoom).FirstOrDefaultAsync();

        if(chatRoom == null){return false;}

        MediaChatItem newMedia = new()
        {
            UserName = mediaRequest.UserName,
            Type = "Media",
            MediaUrls = new()
        };
        // Xử lý danh sách file tải lên
        if (mediaRequest.Files != null)
        {

                List<string> fileUrls = await UploadToStoreS3.CloudinaryService.UploadsToStorageAsync(mediaRequest.Files);
                //var fileUrls = await UploadToStoreS3.BackblazeB2Service.UploadFilesAsync(mediaRequest.Files);
                if (fileUrls != null)
                {   
                    newMedia.MediaUrls.AddRange(fileUrls);
                }
            }
        
        await _chatHub.SendMedia(mediaRequest.UserName, newMedia);

        UpdateResult updateResult = await SaveChatItem.Save(_chatRooms,chatRoom,newMedia);

        return updateResult.ModifiedCount > 0;
    }
}