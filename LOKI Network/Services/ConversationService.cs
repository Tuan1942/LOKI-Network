using LOKI_Model.Enums;
using LOKI_Model.Models;
using LOKI_Network.DbContexts;
using LOKI_Network.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Reflection;

namespace LOKI_Network.Services
{
    public class ConversationService : IConversationService
    {
        private readonly LokiContext _dbContext;
        private readonly IFileService _fileService;
        public ConversationService(LokiContext lokiContext)
        {
            _dbContext = lokiContext;
        }
        public async Task<List<ConversationDTO>> GetConversationsAsync(Guid userId)
        {
            var conversations = await _dbContext.Conversations
                .Where(c => c.ConversationParticipants
                .Any(cp => cp.UserId == userId))
                .Include(c => c.ConversationParticipants)
                .ThenInclude(cp => cp.User)
                .Select(c => new ConversationDTO
                {
                    ConversationId = c.ConversationId,
                    Name = c.ConversationName,
                    IsGroup = c.IsGroup,
                    CreatedDate = c.CreatedDate,
                    Users = new ObservableCollection<UserDTO>(
                        c.ConversationParticipants.Select(cp => new UserDTO
                        {
                            UserId = cp.User.UserId,
                            Username = cp.User.Username,
                            Email = cp.User.Email,
                            ProfilePictureUrl = cp.User.ProfilePictureUrl,
                            Gender = cp.User.Gender,
                        }))
                })
                .ToListAsync();

            return conversations;
        }
        public async Task<List<ConversationDTO>> GetDirectConversationsAsync(Guid userId)
        {
            var conversations = await _dbContext.Conversations
                .Where(c => c.ConversationParticipants.Any(cp => cp.UserId == userId) && c.IsGroup == false)
                .Include(c => c.ConversationParticipants)
                .ThenInclude(cp => cp.User)
                .Select(c => new ConversationDTO
                {
                    ConversationId = c.ConversationId,
                    Name = c.ConversationName,
                    IsGroup = c.IsGroup,
                    CreatedDate = c.CreatedDate,
                    Users = new ObservableCollection<UserDTO>(
                        c.ConversationParticipants.Select(cp => new UserDTO
                        {
                            UserId = cp.User.UserId,
                            Username = cp.User.Username,
                            Email = cp.User.Email,
                            ProfilePictureUrl = cp.User.ProfilePictureUrl,
                            Gender = cp.User.Gender,
                        }))
                })
                .ToListAsync();

            return conversations;
        }
        public async Task<List<ConversationDTO>> GetGroupConversationsAsync(Guid userId)
        {
            var conversations = await _dbContext.Conversations
                .Where(c => c.ConversationParticipants.Any(cp => cp.UserId == userId) && c.IsGroup == true)
                .Include(c => c.ConversationParticipants)
                .ThenInclude(cp => cp.User)
                .Select(c => new ConversationDTO
                {
                    ConversationId = c.ConversationId,
                    Name = c.ConversationName,
                    IsGroup = c.IsGroup,
                    CreatedDate = c.CreatedDate,
                    Users = new ObservableCollection<UserDTO>(
                        c.ConversationParticipants.Select(cp => new UserDTO
                        {
                            UserId = cp.User.UserId,
                            Username = cp.User.Username,
                            Email = cp.User.Email,
                            ProfilePictureUrl = cp.User.ProfilePictureUrl,
                            Gender = cp.User.Gender,
                        }))
                })
                .ToListAsync();

            return conversations;
        }
        public async Task<List<UserDTO>> GetParticipants(Guid conversationId)
        {
            var conversation = await _dbContext.Conversations.Include(c => c.ConversationParticipants).FirstOrDefaultAsync(c => c.ConversationId == conversationId);
            var userList = conversation?.ConversationParticipants.Select(cp => new UserDTO
            {
                UserId = cp.User.UserId,
                Username = cp.User.Username,
                Email = cp.User.Email,
                ProfilePictureUrl = cp.User.ProfilePictureUrl,
                Gender = cp.User.Gender,
            }).ToList();
            return userList;
        }
        public async Task CreateConversation(List<Guid> users, string conversationName)
        {
            if (users == null || users.Count < 2)
            {
                throw new ArgumentException("At least two users are required to create a conversation.");
            }

            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                ConversationName = string.IsNullOrWhiteSpace(conversationName) ? "Unnamed Conversation" : conversationName,
                IsGroup = users.Count > 2,
                CreatedDate = DateTime.UtcNow,
                ConversationParticipants = users.Select((u, index) => new ConversationParticipant
                {
                    ConversationParticipantId = Guid.NewGuid(),
                    UserId = u,
                    Role = users.Count > 2 && index == 0 ? ParticipantRole.Admin : ParticipantRole.None, // Role = Admin to the first user
                    JoinedDate = DateTime.UtcNow
                }).ToList()
            };

            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();
        }
        public async Task LeaveConversation(Guid userId, Guid conversationId)
        {
            try
            {
                // Load the conversation with its participants
                var conversation = await _dbContext.Conversations
                    .Include(c => c.ConversationParticipants)
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

                if (conversation == null)
                {
                    throw new Exception("Conversation not found");
                }

                // Find the participant record for the user
                var participant = conversation.ConversationParticipants
                    .FirstOrDefault(p => p.UserId == userId);

                if (participant == null)
                {
                    throw new Exception("You are not part of this conversation");
                }

                // Remove the participant
                _dbContext.ConversationParticipants.Remove(participant);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) { throw; }
        }
        public async Task<List<AttachmentDTO>> GetAttachmentsByConversationAsync(Guid conversationId)
        {
            var attachments = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .SelectMany(m => m.Attachments)
                .Select(a => new AttachmentDTO
                {
                    AttachmentId = a.AttachmentId,
                    CreatedDate = a.CreatedDate,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileUrl = a.FileUrl
                })
                .ToListAsync();

            return attachments;
        }
        public async Task<List<MessageDTO>> GetMessagesByConversationAsync(Guid conversationId, int pageNumber = 1, int pageSize = 10)
        {
            // Validate inputs
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var messages = _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentDate) // Optional: Order by sent date (most recent first)
                .Skip((pageNumber - 1) * pageSize)  // Skip messages from previous pages
                .Take(pageSize)                     // Take only the current page's messages
                .ToList();

            return messages.Select(m => new MessageDTO
            {
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
                Content = m.Content,
                SentDate = m.SentDate,
                Attachments = m.Attachments?.Select(a => new AttachmentDTO
                {
                    AttachmentId = a.AttachmentId
                }).ToList()
            }).ToList();
        }
        public async Task SendMessage(MessageDTO inputMessage)
        {
            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = inputMessage.SenderId,
                Content = inputMessage.Content,
                SentDate = DateTime.UtcNow
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            // Handle each file in the list
            foreach (var file in inputMessage.Files)
            {
                FileType fileType = FileType.Other;
                var fileUrl = await _fileService.UploadFileAsync(file, fileType);

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
        }
    }
}
