using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.Models;
using LOKI_Client.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.UIs.ViewModels.Account
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool loginPageVisible;

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            WeakReferenceMessenger.Default.Register<LoginRequest>(this, async (r, action) => { await LoginAsync(action.User); });
        }

        public RelayCommand LoginCommand => new RelayCommand(async () => await LoginAsync(new User { Username = Username, Password = Password }));
        public RelayCommand RegisterCommand => new RelayCommand(async () => await RegisterAsync(new User { Username = Username, Password = Password }));
        public RelayCommand SwapPageCommand => new RelayCommand(SwapPage);

        private async Task RegisterAsync(User user)
        {
            IsLoading = true;
            StatusMessage = "Logging in...";

            try
            {
                if (await _userService.Register(user))
                {
                    StatusMessage = "Register successful";
                    SwapPage();
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

        private void SwapPage()
        {
            LoginPageVisible = !LoginPageVisible;
        }

        private async Task LoginAsync(User user)
        {
            IsLoading = true;
            StatusMessage = "Logging in...";

            try
            {
                user = await _userService.Login(user);

                if (user != null)
                {
                    StatusMessage = "Login successful! Connecting to WebSocket...";
                    ConnectWebSocket(user);
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
        private void ConnectWebSocket(User user)
        {
            WeakReferenceMessenger.Default.Send(new ConnectWebSocketRequest(user));
        }
    }
}
