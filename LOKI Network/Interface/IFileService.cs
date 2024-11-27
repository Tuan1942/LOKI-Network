using LOKI_Model.Enums;
using LOKI_Model.Models;

namespace LOKI_Network.Interface
{
    public interface IFileService
    {
        Task<(string FilePath, FileType FileType)> UploadFileAsync(IFormFile file);
        Task<string> GetFileUrl(Guid attachmentId);
        bool DeleteFile(string filePath);
    }
}
