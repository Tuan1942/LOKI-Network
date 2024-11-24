using LOKI_Model.Enums;

namespace LOKI_Network.Helpers
{
    public static class FileHelper
    {
        public static FileType GetFileType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return FileType.Other;
            }

            if (contentType.StartsWith("image/"))
            {
                return FileType.Image;
            }
            else if (contentType.StartsWith("video/"))
            {
                return FileType.Video;
            }
            else if (contentType.StartsWith("audio/"))
            {
                return FileType.Audio;
            }
            else if (contentType.StartsWith("application/") ||
                     contentType.StartsWith("text/"))
            {
                return FileType.Document;
            }
            else
            {
                return FileType.Other;
            }
        }
        public static string GetContentType(string path)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
