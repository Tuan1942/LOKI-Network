using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using LOKI_Model.Models;
using Microsoft.AspNetCore.Http;

namespace LOKI_Client.ApiClients.Interfaces
{
    public interface IConversationService
    {
        Task<List<ConversationDTO>> GetConversationsAsync();
        Task<List<UserDTO>> GetParticipantsAsync(Guid conversationId);
        Task<List<AttachmentDTO>> GetAttachmentsByConversationAsync(Guid conversationId);
        Task<List<MessageDTO>> GetMessagesByConversationAsync(Guid conversationId, int page);
        Task CreateConversationAsync(List<Guid> users, string conversationName);
        Task LeaveConversationAsync(Guid conversationId);
        Task SendMessageAsync(Guid conversationId, MessageDTO message, List<IFormFile> files);
        Task<List<MessageDTO>> GetNextMessagesAsync(Guid conversationId, Guid messageId);
    }
}
