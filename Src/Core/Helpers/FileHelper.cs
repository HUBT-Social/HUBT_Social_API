using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
namespace HUBT_Social_API.Src.Core.Helper
{
    public static class FileHelper
    {
        public static List<IFormFile> ConvertBase64sToFormFiles(this List<string> base64)
        {
            List<IFormFile> result = new List<IFormFile>();
            foreach (var file in base64)
            {
                 // Giải mã Base64 thành byte array
                byte[] fileBytes = Convert.FromBase64String(file);

                // Chuyển đổi thành MemoryStream
                var stream = new MemoryStream(fileBytes);
                result.Add(new FormFile(stream, 0, fileBytes.Length, "file", "unknown"));
            }
            // Tạo IFormFile từ MemoryStream (không cần fileName)
            return result;
        }
        public static IFormFile ConvertBase64ToFormFile(string base64)
        {
                 // Giải mã Base64 thành byte array
                byte[] fileBytes = Convert.FromBase64String(base64);

                // Chuyển đổi thành MemoryStream
                var stream = new MemoryStream(fileBytes);
                return new FormFile(stream, 0, fileBytes.Length, "file", "unknown");
        }
    }
}

