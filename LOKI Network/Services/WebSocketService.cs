using LOKI_Network.DTOs;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace LOKI_Network.Services
{
    public class WebSocketService
    {
        private readonly ConcurrentDictionary<Guid, List<WebSocket>> _userConnections = new();
        // Add a new WebSocket connection for a user
        public void AddConnection(Guid userId, WebSocket webSocket)
        {
            if (!_userConnections.ContainsKey(userId))
            {
                _userConnections[userId] = new List<WebSocket>();
            }

            _userConnections[userId].Add(webSocket);
        }

        // Remove a WebSocket connection for a user
        public void RemoveConnection(Guid userId, WebSocket webSocket)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(webSocket);
                if (!connections.Any())
                {
                    _userConnections.TryRemove(userId, out _);
                }
            }
        }

        // Broadcast a message to all WebSocket connections for a user
        public async Task SendMessageToUserAsync(Guid userId, WebSocketMessage message)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                var content = JsonSerializer.Serialize(message);
                var buffer = Encoding.UTF8.GetBytes(content);
                var tasks = connections.Where(ws => ws.State == WebSocketState.Open)
                                        .Select(ws => ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));

                await Task.WhenAll(tasks);
            }
        }

        // Broadcast a message to all participants in a conversation
        public async Task BroadcastMessageAsync(Guid conversationId, string message, Func<Guid, List<Guid>> getParticipantIds)
        {
            var participantIds = getParticipantIds(conversationId);

            foreach (var userId in participantIds)
            {
                await SendMessageToUserAsync(userId, new WebSocketMessage { });
            }
        }

        // Handle incoming messages for a specific WebSocket
        public async Task ListenToWebSocketAsync(Guid userId, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message from {userId}: {message}");

                    // Example: Echo the message back to the user
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Echo: {message}")), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"WebSocket closed by {userId}");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    RemoveConnection(userId, webSocket);
                }
            }
        }
    }
}
