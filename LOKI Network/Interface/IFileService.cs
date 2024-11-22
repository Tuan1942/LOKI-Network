using LOKI_Model.Enums;

namespace LOKI_Network.Interface
{
    public interface IFileService
    {
        Task<string> UploadFile(Guid messageId, Stream fileStream, string fileName, FileType fileType);
        Task<string> GetFileUrl(Guid attachmentId);
        Task DeleteFile(Guid attachmentId);

    }
}
