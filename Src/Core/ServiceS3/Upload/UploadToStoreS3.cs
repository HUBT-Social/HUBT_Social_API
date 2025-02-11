using System.Net;
using B2Net;
using B2Net.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace HUBT_Social_API.Core.Service.Upload;

public static class UploadToStoreS3
{
    public static class CloudinaryService
    {
        private static Cloudinary _cloudinary;

        // Phương thức khởi tạo Cloudinary
        public static void ConfigureCloudinary(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        // Hàm upload file (sử dụng _cloudinary đã được khởi tạo)
        public static async Task<List<FileUploadResult>> UploadsToStorageAsync(List<IFormFile> files)
        {
            var uploadResult = new List<FileUploadResult>();

            foreach (var file in files)
            {
                var result = await UploadToStorageAsync(file);
                if (result != null) uploadResult.Add(result);
            }

            return uploadResult;
        }

        public static async Task<FileUploadResult?> UploadToStorageAsync(IFormFile file)
        {
            try
            {
                if (file.Length <= 0) return null;

                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                // Thực hiện upload
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Kiểm tra kết quả upload
                if (uploadResult?.StatusCode == HttpStatusCode.OK)
                    return new FileUploadResult
                    {
                        Url = uploadResult.Url?.ToString(),
                        ResourceType = uploadResult.ResourceType
                    };

                // Nếu upload không thành công, trả về null
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<string?> UpdateAvatarAsync(string filePath, IFormFile file)
        {
            if (file.Length <= 0) return null;

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = filePath, // Cố định PublicId
                Overwrite = true // Ghi đè ảnh cũ
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult?.SecureUrl.ToString(); // Trả về URL đầy đủ của ảnh gốc
        }
    }


    public static class BackblazeB2Service
    {
        private static B2Client _client;
        private static string _keyId;
        private static string _applicationKey;
        private static string _bucketId;
        private static string _bucketName;

        public static void InitBackblazeB2Service(B2Client client, string keyId, string applicationKey, string bucketId,
            string bucketName)
        {
            _client = client;
            _keyId = keyId;
            _applicationKey = applicationKey;
            _bucketId = bucketId;
            _bucketName = bucketName;
        }

        public static async Task<List<string>> UploadFilesAsync(List<IFormFile> files)
        {
            var uploadedUrls = new List<string>();

            try
            {
                // Xác thực với Backblaze B2
                await B2Client.AuthorizeAsync(_keyId, _applicationKey);

                foreach (var file in files)
                    if (file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray(); // Chuyển tệp thành byte array
                        var fileName = file.FileName;

                        // Tải tệp lên bằng byte array
                        var fileInfo = await _client.Files.Upload(fileData, fileName, _bucketId);

                        // Tạo URL công khai
                        var baseUrl = $"https://f005.backblazeb2.com/file/{_bucketName}";
                        var fileUrl = $"{baseUrl}/{fileInfo.FileName}";

                        uploadedUrls.Add(fileUrl);
                    }

                return uploadedUrls;
            }
            catch (B2Exception ex)
            {
                Console.WriteLine($"Backblaze B2 error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}