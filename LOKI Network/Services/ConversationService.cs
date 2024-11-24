using LOKI_Model.Enums;
using LOKI_Model.Models;
using LOKI_Network.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Reflection;

namespace LOKI_Network.Services
{
    public class ConversationService : IConversationService
    {
        private readonly LokiContext _context;
        public ConversationService(LokiContext lokiContext)
        {
            _context = lokiContext;
        }
        public async Task<List<ConversationDTO>> GetConversationsAsync(Guid userId)
        {
            var conversations = await _context.Conversations
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
            var conversations = await _context.Conversations
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
            var conversations = await _context.Conversations
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
            var conversation = await _context.Conversations.Include(c => c.ConversationParticipants).FirstOrDefaultAsync(c => c.ConversationId == conversationId);
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

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }
        public async Task LeaveConversation(Guid userId, Guid conversationId)
        {
            try
            {
                // Load the conversation with its participants
                var conversation = await _context.Conversations
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
                _context.ConversationParticipants.Remove(participant);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw; }
        }
        public async Task<List<AttachmentDTO>> GetAttachmentsByConversationAsync(Guid conversationId)
        {
            var attachments = await _context.Messages
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
    }
}
