using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;
using HUBT_Social_API.Features.Chat.DTOs;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
        var (text, links) = ExtractLinksIfPresent(chatRequest.Message);
        
        // Tạo một tin nhắn mới
        MessageModel newMessage = new()
        {
            UserId = chatRequest.UserId,
            Content = new List<string>(),
            Type = links.Any() ? MessageType.MessageLink : MessageType.Message
        };
        
        newMessage.Content.Add(text);
        if(links.Any())
        {
            newMessage.Content.AddRange(links);
        }




        // Cập nhật vào MongoDB
        var update = Builders<ChatRoomModel>
            .Update.Push(cr => cr.Messages, newMessage);

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRequest.GroupId, update);

        return result.ModifiedCount > 0;
    }
    public async Task<LinkMetadata?> FetchLinkMetadataAsync(string url)
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

            return new LinkMetadata
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



    private (string CleanMessage, List<string> Links) ExtractLinksIfPresent(string message)
    {
        if (!Regex.IsMatch(message, @"(http|https):\/\/[^\s]+|www\.[^\s]+"))
        {
            return (message, new List<string>()); // Không có link
        }

        // Xử lý tách link nếu phát hiện
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var links = new List<string>();
        var cleanMessage = new List<string>();

        foreach (var word in words)
        {
            if (Uri.TryCreate(word, UriKind.Absolute, out Uri? uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                links.Add(word);
            }
            else
            {
                cleanMessage.Add(word);
            }
        }

        return (string.Join(" ", cleanMessage), links);
    }
    public async Task<bool> UploadFileAsync(FileRequest chatRequest)
    {
        // Tạo một tin nhắn mới
        MessageModel newMessage = new()
        {
            UserId = chatRequest.UserId,
            Content = new List<string>(),
            Type = MessageType.File
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