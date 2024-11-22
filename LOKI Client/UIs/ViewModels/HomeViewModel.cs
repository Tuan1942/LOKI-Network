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

namespace LOKI_Client.UIs.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {

        #region Properties

        private readonly WebSocketService _webSocketService;
        private UserDTO localUser { get; set; }
        private UserDTO LocalUser
        {
            get { return localUser; }
            set
            {
                localUser = value;
                OnPropertyChanged(nameof(LocalUser));
                OnPropertyChanged(nameof(LoginPageVisible));
            }
        }
        public bool LoginPageVisible
        {
            get
            {
                return LocalUser == null;
            }
        }

        #endregion

        #region Command

        public RelayCommand LogoutCommand => new RelayCommand(async () => await LogoutAsync());

        #endregion

        public HomeViewModel(WebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
            WeakReferenceMessenger.Default.Register<ConnectWebSocketRequest>(this, async (r, action) => { await ConnectWebSocket(action.User); });
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
                var userJson = ApplicationSettingsHelper.GetSetting(nameof(LocalUser));
                LocalUser = JsonSerializer.Deserialize<UserDTO>(userJson);
                await ConnectWebSocket(LocalUser);
            }
            catch (Exception ex)
            {
                // ReLogin fail
                WeakReferenceMessenger.Default.Send(new OpenLoginPageRequest());
            }
        }

        private async Task SendMessageAsync(string message)
        {
            await _webSocketService.SendMessageAsync(message);
        }

        private async Task ConnectWebSocket(UserDTO user)
        {
            LocalUser = user;
            var content = JsonSerializer.Serialize(localUser);
            ApplicationSettingsHelper.SaveSetting(nameof(LocalUser), content);

            await _webSocketService.ConnectAsync(LocalUser.Token);
            WeakReferenceMessenger.Default.Send(new RefreshFriendListRequest(user.Token));
            await _webSocketService.ReceiveMessagesAsync();

            LocalUser = null;
        }

        #endregion

        private async Task LogoutAsync()
        {
            LocalUser = null;
            ApplicationSettingsHelper.SaveSetting(nameof(LocalUser), string.Empty);
            await _webSocketService.CloseAsync();
        }
    }

}
