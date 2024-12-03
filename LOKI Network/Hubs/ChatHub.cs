using LOKI_Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LOKI_Network.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Broadcast a message to specific participants in a conversation
        public async Task BroadcastMessage(MessageDTO message, List<Guid> participantIds)
        {
            foreach (var userId in participantIds)
            {
                await Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message);
            }
        }

        // Example: Send a private message to a user
        public async Task SendMessageToUser(Guid userId, WebSocketMessageDTO message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceivePrivateMessage", message);
        }

        // Optional: Handle client disconnect
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
