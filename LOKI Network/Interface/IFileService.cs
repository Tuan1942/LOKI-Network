using LOKI_Model.Enums;
using LOKI_Model.Models;

namespace LOKI_Network.Interface
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, FileType fileType);
        Task<string> GetFileUrl(Guid attachmentId);
        bool DeleteFile(string filePath);
        Task<List<AttachmentDTO>> GetAttachmentsByConversationAsync(Guid conversationId);
    }
}
