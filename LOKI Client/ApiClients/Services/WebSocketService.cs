using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.ApiClients.Services
{
    public class WebSocketService
    {
        private readonly Uri _baseWebSocketUri;
        private ClientWebSocket? _webSocket;

        public WebSocketService(Uri baseWebSocketUri)
        {
            _baseWebSocketUri = baseWebSocketUri;
        }

        public async Task ConnectAsync(string token)
        {
            _webSocket = new ClientWebSocket();

            // Add the Authorization header with the token
            _webSocket.Options.SetRequestHeader("Authorization", $"{token}");

            try
            {
                // Build the full WebSocket URI with the user token or ID
                var webSocketUri = new Uri($"{_baseWebSocketUri}?token={token}");

                // Connect to the WebSocket server
                await _webSocket.ConnectAsync(webSocketUri, CancellationToken.None);
                Console.WriteLine("WebSocket connected!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection failed: {ex.Message}");
            }
        }

        public async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_webSocket?.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        Console.WriteLine("WebSocket connection closed.");
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Message received: {message}");
                        // Here you can handle incoming messages as needed
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving WebSocket messages: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_webSocket?.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await _webSocket.SendAsync(segment, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
                Console.WriteLine($"Sent message: {message}");
            }
            else
            {
                Console.WriteLine("WebSocket is not connected.");
            }
        }
    }
}
