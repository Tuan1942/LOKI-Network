using LOKI_Model.Enums;

namespace LOKI_Network.Interface
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, FileType fileType);
        Task<string> GetFileUrl(Guid attachmentId);
        bool DeleteFile(string filePath);
    }
}
