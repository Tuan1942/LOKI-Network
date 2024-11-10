using LOKI_Network.DbContexts;
using LOKI_Network.Interface;
using Microsoft.EntityFrameworkCore;

namespace LOKI_Network.Services
{
    public class FileService : IFileService
    {
        private readonly LokiContext _lokiContext;
        private readonly string _fileStoragePath;

        public FileService(LokiContext lokiContext, string fileStoragePath)
        {
            _lokiContext = lokiContext;
            _fileStoragePath = fileStoragePath;
        }

        public async Task<string> UploadFile(Guid messageId, Stream fileStream, string fileName, FileType fileType)
        {
            var attachmentId = Guid.NewGuid();
            var folderPath = Path.Combine(_fileStoragePath, fileName);
            var filePath = Path.Combine(_fileStoragePath, attachmentId.ToString());

            // Save file to storage
            using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOut);
            }

            // Create and save Attachment metadata
            var attachment = new Attachment
            {
                AttachmentId = attachmentId,
                MessageId = messageId,
                FileUrl = filePath,
                FileType = fileType,
                CreatedDate = DateTime.UtcNow
            };
            _lokiContext.Attachments.Add(attachment);
            await _lokiContext.SaveChangesAsync();

            return filePath;
        }

        public async Task<string> GetFileUrl(Guid attachmentId)
        {
            var attachment = await _lokiContext.Attachments
                .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);

            if (attachment == null)
            {
                throw new FileNotFoundException("Attachment not found.");
            }

            return attachment.FileUrl;
        }

        public async Task DeleteFile(Guid attachmentId)
        {
            var attachment = await _lokiContext.Attachments
                .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);

            if (attachment == null)
            {
                throw new FileNotFoundException("Attachment not found.");
            }

            // Delete file from storage
            if (File.Exists(attachment.FileUrl))
            {
                File.Delete(attachment.FileUrl);
            }

            // Remove attachment record from database
            _lokiContext.Attachments.Remove(attachment);
            await _lokiContext.SaveChangesAsync();
        }
    }
}
