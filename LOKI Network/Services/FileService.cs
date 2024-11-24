using LOKI_Model.Enums;
using LOKI_Model.Models;
using LOKI_Network.DbContexts;
using LOKI_Network.Helpers;
using LOKI_Network.Interface;
using Microsoft.EntityFrameworkCore;

namespace LOKI_Network.Services
{
    public class FileService : IFileService
    {
        private readonly LokiContext _dbContext;
        private readonly string _fileStoragePath;

        public FileService(LokiContext lokiContext, string fileStoragePath)
        {
            _dbContext = lokiContext;
            _fileStoragePath = fileStoragePath;
        }

        public async Task <string> UploadFileAsync(IFormFile file, FileType fileType)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var folderPath = Path.Combine(_fileStoragePath, fileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var contentType = FileHelper.GetContentType(filePath);
            fileType = FileHelper.GetFileType(contentType);
            return filePath;
        }

        public async Task<string> GetFileUrl(Guid attachmentId)
        {
            var attachment = await _dbContext.Attachments
                .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);

            if (attachment == null)
            {
                throw new FileNotFoundException("Attachment not found.");
            }

            return attachment.FileUrl;
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
