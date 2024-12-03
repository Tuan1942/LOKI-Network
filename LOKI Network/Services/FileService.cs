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

        public async Task<(string FilePath, FileType FileType)> UploadFileAsync(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }

            var filePath = Path.Combine(_fileStoragePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var contentType = FileHelper.GetContentType(filePath);
            var fileType = FileHelper.GetFileType(contentType);

            return (filePath, fileType);
        }

        public async Task<(string FileUrl, FileType FileType)> GetFileUrl(Guid attachmentId)
        {
            var attachment = await _dbContext.Attachments
                .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);

            if (attachment == null)
            {
                throw new FileNotFoundException("Attachment not found.");
            }

            return (attachment.FileUrl, attachment.FileType);
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
