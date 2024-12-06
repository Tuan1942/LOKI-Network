using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.Models;
using LOKI_Client.Models.Objects;

namespace LOKI_Client.UIs.ViewModels.Account
{
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserObject user;
        public ProfileViewModel() 
        {
            RegisterService();
        }

        private void RegisterService()
        {
            WeakReferenceMessenger.Default.Register<UpdateProfilePageRequest>(this, async (r, action) => UpdateProfile(action.User));
        }

        private async Task UpdateProfile(UserObject user)
        {
            User = user;
        }
    }
}
