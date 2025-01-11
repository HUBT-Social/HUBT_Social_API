
using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

public class MediaUploadService : IMediaUploadService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    public MediaUploadService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }
    public async Task<bool> UploadMediaAsync(MediaRequest mediaRequest,IHubContext<ChatHub> hubContext,string eventName)
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
        
        await SendingItem.SendChatItem(mediaRequest.GroupId,newMedia,eventName,hubContext);

        UpdateResult updateResult = await SaveChatItem.Save(_chatRooms,chatRoom,newMedia);

        return updateResult.ModifiedCount > 0;
    }
}