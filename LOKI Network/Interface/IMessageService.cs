using LOKI_Network.DbContexts;

namespace LOKI_Network.Interface
{
    public interface IMessageService
    {
        Task<Message> CreateMessageAsync(Guid senderId, string content, Stream fileStream, string fileName, FileType fileType);
        Task DeleteMessageAsync(Guid messageId);
    }
}
