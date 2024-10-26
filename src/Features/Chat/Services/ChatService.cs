using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using HUBTSOCIAL.Src.Features.Chat.Services.IChatServices;

namespace HUBTSOCIAL.Src.Features.Chat.Services
{
    public class ChatService : IChatService
    {
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;
        private readonly IImageService _imageService;

        public ChatService(IMessageService messageService, IFileService fileService, IImageService imageService)
        {
            _messageService = messageService;
            _fileService = fileService;
            _imageService = imageService;
        }


        public async Task<bool> SendMessageAsync(MessageDTO messageDto)
        {
            return await _messageService.SendMessageAsync(messageDto.ChatRoomId, messageDto.UserId, messageDto.Content);
        }

        public async Task<List<MessageModel>?> GetMessagesInChatRoomAsync(string chatRoomId)
        {
            return await _messageService.GetMessagesInChatRoomAsync(chatRoomId);
        }

        public async Task<bool> DeleteMessageAsync(string chatRoomId, string messageId)
        {
            return await _messageService.DeleteMessageAsync(chatRoomId, messageId);
        }

        public async Task<List<MessageModel>> SearchMessagesInChatRoomAsync(string chatRoomId, string keyword)
        {
            return await _messageService.SearchMessagesInChatRoomAsync(chatRoomId, keyword);
        }

        public async Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData)
        {
            return await _imageService.UploadImageAsync(userId, chatRoomId, imageData);
        }

        public async Task<bool> UploadFileAsync(string chatRoomId, byte[] fileData, string fileName)
        {
            return await _fileService.UploadFileAsync(chatRoomId, fileData, fileName);
        }
    }

}
