using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.Models;
using LOKI_Model.Models;
using LOKI_Client.Models.Helper;
using System;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LOKI_Client.Extensions.Authorize;
using Microsoft.Extensions.DependencyInjection;

namespace LOKI_Client.UIs.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {

        #region Properties

        private readonly WebSocketService _webSocketService;
        public readonly SignalRService _signalRService;

        [ObservableProperty]
        private bool loggedIn = true;
        #endregion

        #region Command

        public RelayCommand LogoutCommand => new RelayCommand(async () => await LogoutAsync());

        #endregion

        public HomeViewModel(WebSocketService webSocketService, SignalRService signalRService)
        {
            _webSocketService = webSocketService;
            _signalRService = signalRService;
            WeakReferenceMessenger.Default.Register<ConnectWebSocketRequest>(this, async (r, action) => { await ConnectSignalRAsync(action.Token); });
            RefreshConnection();
            Application.Current.Exit += async (sender, args) =>
            {
                await _webSocketService.CloseAsync();
            };
        }

        #region Navigate


        #endregion

        #region WebSocket

        private async Task RefreshConnection()
        {
            try
            {
                var userProvider = App.Current.Services.GetRequiredService<UserProvider>();
                await ConnectSignalRAsync(userProvider.User?.Token);
            }
            catch (Exception ex)
            {
                LoggedIn = false;
                WeakReferenceMessenger.Default.Send(new OpenLoginPageRequest());
            }
        }

        private async Task SendMessageAsync(string message)
        {
            await _signalRService.SendAsync("SendMessage", new { Content = message });
        }

        private async Task ConnectSignalRAsync(string token)
        {
            try
            {
                await _signalRService.StartAsync();

                // Handle incoming messages
                _signalRService.On<MessageDTO>("ReceiveMessage", message =>
                {
                    WeakReferenceMessenger.Default.Send(new AddMessageRequest(message));
                });

                LoggedIn = true;
                WeakReferenceMessenger.Default.Send(new RefreshConversationListRequest(token));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
                LoggedIn = false;
            }
        }

        #endregion

        private async Task DisconnectSignalRAsync()
        {
            try
            {
                await _signalRService.StopAsync();
                LoggedIn = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disconnecting SignalR: {ex.Message}");
            }
        }

        private async Task LogoutAsync()
        {
            await DisconnectSignalRAsync();
        }
    }

}
