using LOKI_Network.DbContexts;
using LOKI_Network.Interface;
using Microsoft.EntityFrameworkCore;

public class MessageService : IMessageService
{
    private readonly LokiContext _lokiContext;
    private readonly IFileService _fileService;

    public MessageService(LokiContext lokiContext, IFileService fileService)
    {
        _lokiContext = lokiContext;
        _fileService = fileService;
    }

    public async Task<Message> CreateMessageAsync(Guid senderId, string content, Stream fileStream, string fileName, FileType fileType)
    {
        var message = new Message
        {
            MessageId = Guid.NewGuid(),
            SenderId = senderId,
            Content = content,
            SentDate = DateTime.UtcNow
        };

        // Save message to database first
        _lokiContext.Messages.Add(message);
        await _lokiContext.SaveChangesAsync();

        // If there is an attachment, use FileService to handle it
        if (fileStream != null)
        {
            var fileUrl = await _fileService.UploadFile(message.MessageId, fileStream, fileName, fileType);

            var attachment = new Attachment
            {
                AttachmentId = Guid.NewGuid(),
                MessageId = message.MessageId,
                FileUrl = fileUrl,
                FileName = fileName,
                FileType = fileType,
                CreatedDate = DateTime.UtcNow
            };

            _lokiContext.Attachments.Add(attachment);
            await _lokiContext.SaveChangesAsync();
        }

        return message;
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await _lokiContext.Messages
            .Include(m => m.Attachments) 
            .FirstOrDefaultAsync(m => m.MessageId == messageId);

        if (message == null)
        {
            throw new KeyNotFoundException("Message not found.");
        }

        // Delete attachments using FileService
        foreach (var attachment in message.Attachments)
        {
            await _fileService.DeleteFile(attachment.AttachmentId);
        }

        // Delete the message itself
        _lokiContext.Messages.Remove(message);
        await _lokiContext.SaveChangesAsync();
    }
}
