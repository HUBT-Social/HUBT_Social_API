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
using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public class UploadChatServices : IUploadChatServices
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly Cloudinary _cloudinary;
    private readonly IChatHub _chatHub;

    public UploadChatServices(IMongoCollection<ChatRoomModel> chatRooms, Cloudinary cloudinary,IChatHub chatHub)
    {
        _chatRooms = chatRooms;
        _cloudinary = cloudinary;
        _chatHub = chatHub;
    }

    public async Task<bool> UploadMessageAsync(MessageRequest chatRequest)
    {
        if(!_chatRooms.Find(room => room.UserIds.Contains(chatRequest.SenderId)).Any()){return false;}

        var (text, links) = ExtractLinksIfPresent(chatRequest.Content);
        
        // Tạo một tin nhắn mới
        MessageChatItem newMessage = new()
        {
            SenderId = chatRequest.SenderId,
            Content = text,
            Links = new()
        };

        if(links.Count > 0)
        {
            foreach (var link in links)
            {
                var metadata = await FetchLinkMetadataAsync(link);
                if(metadata != null)
                {
                    newMessage.Links.Add(metadata);
                }
            }
        }

        await _chatHub.SendMessage(chatRequest.GroupId, newMessage); 

        // Cập nhật vào MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.ChatItems, newMessage);

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> UploadMediaAsync(FileRequest chatRequest)
    {
        if(!_chatRooms.Find(room => room.UserIds.Contains(chatRequest.SenderId)).Any()){return false;}

        MediaChatItem newMedia = new()
        {
            SenderId = chatRequest.SenderId,
            MediaUrls = new()
        };
        // Xử lý danh sách file tải lên
        if (chatRequest.Files != null)
        {
            foreach (var file in chatRequest.Files)
            {
                var fileUrl = await UploadToStorageAsync(file);
                if (fileUrl != null)
                {
                    newMedia.MediaUrls.Add(fileUrl);
                }
            }
         }
        await _chatHub.SendMedia(chatRequest.SenderId, newMedia);

        // Cập nhật vào MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.ChatItems, newMedia);

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

        return result.ModifiedCount > 0;
    }

    // Hàm này dùng ở khắp nơi CẤM XÓA (DONT DELETE)
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




































    private async Task<LinkMetadataModel?> FetchLinkMetadataAsync(string url)
    {
        try
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; FetchBot/1.0)");

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch URL {url}. Status code: {response.StatusCode}");
                return null;
            }

            var htmlContent = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();
            var thumbnail = doc.DocumentNode
                .SelectSingleNode("//meta[@property='og:image']")?
                .GetAttributeValue("content", "");
            var description = doc.DocumentNode
                .SelectSingleNode("//meta[@name='description']")?
                .GetAttributeValue("content", "") ??
                doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']")?
                .GetAttributeValue("content", "");

            return new LinkMetadataModel
            {
                Url = url,
                Title = title ?? "No Title",
                Thumbnail = thumbnail,
                Description = description ?? "No Description"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch metadata for {url}: {ex.Message}");
            return null;
        }
    }
    private (string Message, List<string> Links) ExtractLinksIfPresent(string message)
    {
        if (!Regex.IsMatch(message, @"(http|https):\/\/[^\s]+|www\.[^\s]+"))
        {
            return (message, new List<string>()); // Không có link
        }

        // Xử lý tách link nếu phát hiện
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var links = new List<string>();
        var messageWithLinks = new List<string>();

        foreach (var word in words)
        {
            if (Uri.TryCreate(word, UriKind.Absolute, out Uri? uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                // Thêm link vào danh sách
                links.Add(word);
                // Chèn thẻ <a> vào từ chứa URL
                messageWithLinks.Add($"<a href=\"{word}\" target=\"_blank\">{word}</a>");
            }
            else
            {
                // Thêm từ không phải link vào chuỗi kết quả
                messageWithLinks.Add(word);
            }
        }

        // Kết hợp các từ lại thành chuỗi sau khi thay thế link thành thẻ <a>
        string updatedMessage = string.Join(" ", messageWithLinks);

        return (updatedMessage, links);
    }
    
    

}