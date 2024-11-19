using LOKI_Network.DbContexts;
using LOKI_Network.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace LOKI_Network.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketService _webSocketService;
        private readonly IConfiguration _configuration;

        public WebSocketMiddleware(RequestDelegate next, WebSocketService webSocketService, IConfiguration configuration)
        {
            _next = next;
            _webSocketService = webSocketService;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var token = context.Request.Headers["Authorization"].ToString();
                var user = await UserService.ValidateJwtToken(token, _configuration);
                if (user == null)  // Validate token
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return;
                }

                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var userId = user.UserId;

                _webSocketService.AddConnection(userId, webSocket);

                await HandleWebSocketConnection(webSocket, userId);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket, Guid userId)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // Close the WebSocket connection and remove it from the service
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        _webSocketService.RemoveConnection(userId, webSocket);
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Handle incoming message (could be routing to conversation logic, etc.)
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received message: {message}");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in WebSocket connection: {ex.Message}");
                    break;
                }
            }

            // Clean up on disconnect
            _webSocketService.RemoveConnection(userId, webSocket);
            webSocket.Dispose();
        }
    }
}
