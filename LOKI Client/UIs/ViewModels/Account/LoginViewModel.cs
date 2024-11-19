using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.Models.DTOs;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LOKI_Client.UIs.ViewModels.Account
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private ClientWebSocket? _webSocket;
        private string _webSocketUri = "wss://localhost:3000/ws";

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private bool isLoading;

        public RelayCommand LoginCommand => new RelayCommand(async () => await LoginAsync());

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
        }

        private async Task LoginAsync()
        {
            IsLoading = true;
            StatusMessage = "Logging in...";

            try
            {
                var user = new User { Username = Username, Password = Password };
                var token = await _userService.Login(user);

                if (!string.IsNullOrEmpty(token))
                {
                    StatusMessage = "Login successful! Connecting to WebSocket...";
                    await ConnectWebSocket(token); // Connect to WebSocket after successful login
                }
                else
                {
                    StatusMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ConnectWebSocket(string token)
        {
            _webSocket = new ClientWebSocket();

            try
            {
                // Add the Authorization header with the token
                _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");

                // Build the WebSocket URI
                var webSocketUri = new Uri($"{_webSocketUri}"); // Replace _webSocketUri with your WebSocket server URI

                // Connect to the WebSocket server
                await _webSocket.ConnectAsync(webSocketUri, CancellationToken.None);

                StatusMessage = "Connected to WebSocket!";

                // Start receiving messages from the WebSocket server
                await ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"WebSocket connection failed: {ex.Message}";
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        StatusMessage = "WebSocket connection closed.";
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        HandleMessage(message); // Handle incoming message
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void HandleMessage(string message)
        {
            StatusMessage = message;
            // Assuming the message is in JSON format, deserialize it (e.g., using JSON.NET or System.Text.Json)
            //var messageObj = System.Text.Json.JsonSerializer.Deserialize<WebSocketMessage>(message);
            //switch (messageObj?.Type)
            //{
            //    case "refresh":
            //        // Handle refresh action
            //        StatusMessage = "Refreshing messages...";
            //        break;

            //    case "notification":
            //        // Handle notification
            //        StatusMessage = $"Notification: {messageObj.Content}";
            //        break;

            //    default:
            //        StatusMessage = $"Unknown message type: {messageObj?.Type}";
            //        break;
            //}
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

    // WebSocket message DTO
    public class WebSocketMessage
    {
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
