using LOKI_Client.Models.Objects;
using Microsoft.AspNetCore.Http;

namespace LOKI_Client.ApiClients.Interfaces
{
    public interface IConversationService
    {
        Task<List<ConversationObject>> GetConversationsAsync();
        Task<List<UserObject>> GetParticipantsAsync(Guid conversationId);
        Task<List<AttachmentObject>> GetAttachmentsByConversationAsync(Guid conversationId);
        Task<List<MessageObject>> GetMessagesByConversationAsync(Guid conversationId, int page);
        Task CreateConversationAsync(List<Guid> users, string conversationName);
        Task LeaveConversationAsync(Guid conversationId);
        Task SendMessageAsync(Guid conversationId, MessageObject message, List<IFormFile> files);
        Task<List<MessageObject>> GetNextMessagesAsync(Guid conversationId, Guid messageId);
    }
}
