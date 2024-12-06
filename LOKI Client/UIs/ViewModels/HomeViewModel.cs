using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.Models;
using System.Windows;
using LOKI_Client.Models.Objects;
using LOKI_Client.Extensions.Authorize;
using Microsoft.Extensions.DependencyInjection;

namespace LOKI_Client.UIs.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        #region Properties

        public readonly SignalRService _signalRService;
        private AuthorizationHandler _authorizationHandler;

        [ObservableProperty]
        private bool loggedIn = false;

        [ObservableProperty]
        private bool profilePageOpened;
        #endregion

        #region Command

        [RelayCommand]
        private async Task Logout()
        {
            await DisconnectSignalRAsync();
        }


        [RelayCommand]
        private void CloseProfilePage()
        {
            ProfilePageOpened = false;
        }

        [RelayCommand]
        private async Task OpenProfilePage(UserObject user)
        {
            ProfilePageOpened = true;
        }


        #endregion

        public HomeViewModel(SignalRService signalRService)
        {
            _signalRService = signalRService;
            _authorizationHandler = App.Current.Services.GetRequiredService<AuthorizationHandler>();
            RefreshConnection();
            RegisterService();
        }

        private void RegisterService()
        {
            WeakReferenceMessenger.Default.Register<ConnectSignalRRequest>(this, async (r, action) => { await ConnectSignalRAsync(); });
            WeakReferenceMessenger.Default.Register<UpdateProfilePageRequest>(this, async (r, action) => { await OpenProfilePage(action.User); });
            WeakReferenceMessenger.Default.Register<OpenLoginPageRequest>(this, (r, action) => { LoggedIn = false; });
            Application.Current.Exit += async (sender, args) =>
            {
                await _signalRService.StopAsync();
            };
        }

        #region Navigate


        #endregion

        #region SignalR

        private async Task RefreshConnection()
        {
            try
            {
                await ConnectSignalRAsync();
            }
            catch (Exception ex)
            {
                LoggedIn = false;
            }
        }

        private async Task SendMessageAsync(string message)
        {
            await _signalRService.SendAsync("SendMessage", new { Content = message });
        }

        private async Task ConnectSignalRAsync()
        {
            try
            {
                await _signalRService.StartAsync();

                // Handle incoming messages
                _signalRService.On<MessageObject>("ReceiveMessage", message =>
                {
                    WeakReferenceMessenger.Default.Send(new AddMessageRequest(message));
                });

                LoggedIn = true;
                WeakReferenceMessenger.Default.Send(new RefreshConversationListRequest());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
                LoggedIn = false;
            }
        }

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

        #endregion

    }

}
