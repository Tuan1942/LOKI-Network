using LOKI_Model.Models;
using LOKI_Network.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Reflection;

namespace LOKI_Network.Services
{
    public interface IConversationService
    {
        Task<List<ConversationDTO>> GetConversationsAsync(Guid userId);
        Task<List<ConversationDTO>> GetDirectConversationsAsync(Guid userId);
        Task<List<ConversationDTO>> GetGroupConversationsAsync(Guid userId);
        Task<List<UserDTO>> GetParticipants(Guid conversationId);
        Task CreateConversation(List<Guid> users, string conversationName);
        Task LeaveConversation(Guid userId, Guid conversationId);
        Task<List<AttachmentDTO>> GetAttachmentsByConversationAsync(Guid conversationId);
    }
}