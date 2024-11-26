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

        [ObservableProperty]
        private bool loggedIn = true;
        #endregion

        #region Command

        public RelayCommand LogoutCommand => new RelayCommand(async () => await LogoutAsync());

        #endregion

        public HomeViewModel(WebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
            WeakReferenceMessenger.Default.Register<ConnectWebSocketRequest>(this, async (r, action) => { await ConnectWebSocket(action.Token); });
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
                await ConnectWebSocket(userProvider.User?.Token);
            }
            catch (Exception ex)
            {
                LoggedIn = false;
                WeakReferenceMessenger.Default.Send(new OpenLoginPageRequest());
            }
        }

        private async Task SendMessageAsync(string message)
        {
            await _webSocketService.SendMessageAsync(message);
        }

        private async Task ConnectWebSocket(string token)
        {
            try
            {
                LoggedIn = await _webSocketService.ConnectAsync(token);
                WeakReferenceMessenger.Default.Send(new RefreshConversationListRequest(token));
                await _webSocketService.ReceiveMessagesAsync();
            }
            catch (Exception) { LoggedIn = false; }
        }

        #endregion

        private async Task LogoutAsync()
        {
            await _webSocketService.CloseAsync();
            LoggedIn = false;
        }
    }

}
