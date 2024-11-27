using LOKI_Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Models.Helper
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
        public static string GetMimeType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" or "jfif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" or ".docx" => "application/msword",
                ".xls" or ".xlsx" => "application/vnd.ms-excel",
                ".zip" => "application/zip",
                _ => "application/octet-stream", // Default for unknown types
            };
        }
    }
}
