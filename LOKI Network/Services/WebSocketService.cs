using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace LOKI_Network.Services
{
    public class WebSocketService
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _activeConnections = new();

        public void AddConnection(Guid userId, WebSocket webSocket)
        {
            _activeConnections[userId] = webSocket;
        }

        public void RemoveConnection(Guid userId)
        {
            _activeConnections.TryRemove(userId, out _);
        }

        public async Task BroadcastMessageAsync(Guid conversationId, string message)
        {
            // Retrieve all participant connections in this conversation
            var participantIds = GetParticipantIdsInConversation(conversationId); // Assuming a helper to fetch these

            foreach (var userId in participantIds)
            {
                if (_activeConnections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private List<Guid> GetParticipantIdsInConversation(Guid conversationId)
        {
            // Logic to retrieve participant Ids for this conversation
            return null;
        }
    }
}
