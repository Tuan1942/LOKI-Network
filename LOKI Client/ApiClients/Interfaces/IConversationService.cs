using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using LOKI_Model.Models;

namespace LOKI_Client.ApiClients.Interfaces
{
    public interface IConversationService
    {
        Task<List<ConversationDTO>> GetConversationsAsync(Guid userId);
        Task<List<UserDTO>> GetParticipantsAsync(Guid conversationId);
        Task CreateConversationAsync(List<Guid> users, string conversationName);
        Task LeaveConversationAsync(Guid conversationId, Guid userId);
    }
}
