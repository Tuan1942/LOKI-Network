using LOKI_Model.Models;
using LOKI_Network.DbContexts;

namespace LOKI_Network.Interface
{
    public interface IMessageService
    {
        Task<Guid> CreateMessageAsync(Guid senderId, string content, List<IFormFile> files = null);
        Task<MessageDTO> GetMessageAsync(Guid messageId);
        Task<bool> UpdateMessageAsync(MessageDTO inputMessage);
        Task DeleteMessageAsync(Guid messageId);
        Task<bool> SendMessageAsync(Guid senderId, Guid messageId, Guid conversationId);
    }
}
