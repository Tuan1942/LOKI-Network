using LOKI_Model.Enums;
using LOKI_Network.DbContexts;
using LOKI_Model.Models;
using LOKI_Network.Interface;
using LOKI_Network.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class MessageService : IMessageService
{
    private readonly LokiContext _dbContext;
    private readonly IFileService _fileService;
    private readonly WebSocketService _webSocketService;

    public MessageService(LokiContext lokiContext, IFileService fileService, WebSocketService webSocketService)
    {
        _dbContext = lokiContext;
        _fileService = fileService;
        _webSocketService = webSocketService;
    }

    public async Task<Guid> CreateMessageAsync(Guid senderId, string content, List<IFormFile> files)
    {
        var message = new Message
        {
            MessageId = Guid.NewGuid(),
            SenderId = senderId,
            Content = content,
            SentDate = DateTime.UtcNow
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        // Handle each file in the list
        foreach (var file in files)
        {
            using var fileStream = file.OpenReadStream();
            var fileUrl = await _fileService.UploadFile(message.MessageId, fileStream, file.FileName, FileType.Other);

            var attachment = new Attachment
            {
                AttachmentId = Guid.NewGuid(),
                MessageId = message.MessageId,
                FileUrl = fileUrl,
                FileName = file.FileName,
                FileType = FileType.Other,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.Attachments.Add(attachment);
        }

        await _dbContext.SaveChangesAsync();
        return message.MessageId;
    }
    public async Task<bool> SendMessageAsync(Guid senderId, Guid messageId, Guid conversationId)
    {
        var message = _dbContext.Messages.Find(messageId);
        if (message == null) throw new Exception("Message not found.");
        if (message.ConversationId == Guid.Empty)
        {
            message.ConversationId = conversationId;
        }
        else
        {
            _dbContext.Add(new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = message.SenderId,
                ConversationId = conversationId,
                Content = message.Content,
                SentDate = DateTime.UtcNow
            });
        }
        await _dbContext.SaveChangesAsync();
        //await _webSocketService.BroadcastMessageAsync(conversationId, $"New message from {senderId}");
        return true;
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await _dbContext.Messages
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
        _dbContext.Messages.Remove(message);
        await _dbContext.SaveChangesAsync();
    }

    public List<MessageDTO> GetMessagesByConversation(Guid conversationId)
    {
        var messages = _dbContext.Messages.Where(m => m.ConversationId == conversationId);
        return messages?.Select(m => new MessageDTO()).ToList();
    }
}
