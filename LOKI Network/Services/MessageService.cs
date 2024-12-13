﻿using LOKI_Model.Enums;
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

    public MessageService(LokiContext lokiContext, IFileService fileService)
    {
        _dbContext = lokiContext;
        _fileService = fileService;
    }

    public async Task<MessageDTO> GetMessageAsync(Guid messageId)
    {
        var m = await _dbContext.Messages.Include(m => m.Attachments).FirstAsync(m => m.MessageId == messageId);
        return new MessageDTO
            {
            MessageId = m.MessageId,
                User = new UserDTO
                {
                    Username = m.Sender?.Username,
                    FirstName = m.Sender?.FirstName,
                    LastName = m.Sender?.LastName,
                    MiddleName = m.Sender?.MiddleName,
                    ProfilePictureUrl = m.Sender?.ProfilePictureUrl,
                    Email = m.Sender?.Email,
                    Gender = m.Sender.Gender,
                },
                ConversationId = m.ConversationId,
                Content = m.Content,
                SentDate = m.SentDate,
            Attachments = m.Attachments?.Select(a => new AttachmentDTO
            {
                AttachmentId = a.AttachmentId,
                CreatedDate = a.CreatedDate,
                FileName = a.FileName,
                FileType = a.FileType,
                FileUrl = a.FileUrl
            }).ToList()
            };
    }

    public async Task<Guid> CreateMessageAsync(Guid senderId, string content, List<IFormFile> files = null)
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
            FileType fileType = FileType.Other;
            var fileResult = await _fileService.UploadFileAsync(file);

            var attachment = new Attachment
            {
                AttachmentId = Guid.NewGuid(),
                MessageId = message.MessageId,
                FileUrl = fileResult.FilePath,
                FileName = file.FileName,
                FileType = FileType.Other,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.Attachments.Add(attachment);
        }

        await _dbContext.SaveChangesAsync();
        return message.MessageId;
    }

    public async Task<bool> UpdateMessageAsync(MessageDTO inputMessage)
    {
        var message = await _dbContext.FindAsync<Message>(inputMessage.MessageId);
        if (message == null) throw new Exception("Message not found.");
        message.Content = inputMessage.Content;
        return true;
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
            _fileService.DeleteFile(attachment.FileUrl);
        }

        // Delete the message itself
        _dbContext.Messages.Remove(message);
        await _dbContext.SaveChangesAsync();
    }

}

