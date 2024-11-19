using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
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
        private readonly WebSocketService _webSocketService;

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

        public LoginViewModel(IUserService userService, WebSocketService webSocketService)
        {
            _userService = userService;
            _webSocketService = webSocketService;
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
                    await _webSocketService.ConnectAsync(token);
                    await _webSocketService.ReceiveMessagesAsync();
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

        public async Task SendMessageAsync(string message)
        {
            await _webSocketService.SendMessageAsync(message);
        }
    }

    // WebSocket message DTO
    public class WebSocketMessage
    {
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
