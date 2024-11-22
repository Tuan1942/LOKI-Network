using LOKI_Model.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace LOKI_Network.Interface
{
    public interface IWebSocketService
    {
        void AddConnection(Guid userId, WebSocket webSocket);

        void RemoveConnection(Guid userId, WebSocket webSocket);
        Task SendMessageToUserAsync(Guid userId, WebSocketMessageDTO message);
        Task BroadcastMessageAsync(Guid conversationId, string message, Func<Guid, List<Guid>> getParticipantIds);
        Task ListenToWebSocketAsync(Guid userId, WebSocket webSocket);
    }
}
