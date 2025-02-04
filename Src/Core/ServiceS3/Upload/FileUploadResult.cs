namespace HUBT_Social_API.Core.Service.Upload;
public class FileUploadResult
    {
        public string Url { get; set; } // URL của file
        public string ResourceType { get; set; } // Loại file (image, video, raw,...)
    }