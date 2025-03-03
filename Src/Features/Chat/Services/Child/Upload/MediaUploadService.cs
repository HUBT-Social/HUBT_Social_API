
using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

public class MediaUploadService : IMediaUploadService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly IMongoCollection<ChatHistory> _chatHistory;
    public MediaUploadService(IMongoCollection<ChatRoomModel> chatRooms,IMongoCollection<ChatHistory> chatHistory)
    {
        _chatRooms = chatRooms;
        _chatHistory = chatHistory;
    }
    public async Task<bool> UploadMediaAsync(MediaRequest mediaRequest,IHubContext<ChatHub> hubContext)
    {
        
        // Lấy ChatRoom từ MongoDB
        FilterDefinition<ChatRoomModel> filterGetChatRoom = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, mediaRequest.GroupId);
        ChatRoomModel chatRoom = await _chatRooms.Find(filterGetChatRoom).FirstOrDefaultAsync();

        if(chatRoom == null){return false;}

        
        List<FilePaths> FilePaths = new List<FilePaths>();
        

        // Xử lý danh sách file tải lên
        if (mediaRequest.Medias != null)
        {

                List<FileUploadResult> fileUrls = await UploadToStoreS3.CloudinaryService.UploadsToStorageAsync(mediaRequest.Medias);
                //var fileUrls = await UploadToStoreS3.BackblazeB2Service.UploadFilesAsync(mediaRequest.Files);
                if (fileUrls != null)
                {
                    foreach (var item in fileUrls)
                    {
                        FilePaths newFilePath = new FilePaths
                            {
                                Url = item.Url,
                                Type = item.ResourceType
                            };
                        FilePaths.Add(newFilePath);
                    }   
       
                    
                }
            }

        MessageModel message = await MessageModel.CreateMediaMessageAsync(chatRoom.CurrentBlockId,mediaRequest.UserId,FilePaths,mediaRequest.ReplyToMessage);
        
        await SendingItem.SendChatItem(mediaRequest.GroupId,message,hubContext);

        UpdateResult updateResult = await SaveChatItem.Save(_chatRooms,_chatHistory,chatRoom.Id,message);

        return updateResult.ModifiedCount > 0;
    }
}