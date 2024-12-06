using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.Extensions.Authorize;
using LOKI_Client.Models;
using LOKI_Client.Models.Objects;

namespace LOKI_Client.UIs.ViewModels.Account
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly UserProvider _userProvider;

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

        public LoginViewModel(IUserService userService, UserProvider userProvider)
        {
            _userService = userService;
            _userProvider = userProvider;
            WeakReferenceMessenger.Default.Register<LoginRequest>(this, async (r, action) => { await LoginAsync(action.User); });
            WeakReferenceMessenger.Default.Register<OpenLoginPageRequest>(this, (r, action) => { Username = string.Empty; Password = string.Empty; });
        }

        public RelayCommand LoginCommand => new RelayCommand(async () => await LoginAsync(new UserObject { Username = Username, Password = Password }));
        public RelayCommand RegisterCommand => new RelayCommand(async () => await RegisterAsync(new UserObject { Username = Username, Password = Password }));
        public RelayCommand SwapPageCommand => new RelayCommand(SwapPage);

        private async Task RegisterAsync(UserObject user)
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

        private async Task LoginAsync(UserObject user)
        {
            IsLoading = true;
            StatusMessage = "Logging in...";

            try
            {
                user = await _userService.Login(user);

                if (user != null)
                {
                    StatusMessage = "Login successful! Connecting to WebSocket...";
                    _userProvider.User = user;
                    ConnectWebSocket();
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
        private void ConnectWebSocket()
        {
            WeakReferenceMessenger.Default.Send(new ConnectSignalRRequest());
        }
    }
}
