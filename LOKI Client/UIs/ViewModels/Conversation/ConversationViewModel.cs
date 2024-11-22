using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.Models;
using LOKI_Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.UIs.ViewModels.Conversation
{
    public partial class ConversationViewModel : ObservableObject
    {
        private readonly FriendshipService _friendshipService;

        [ObservableProperty]
        private ObservableCollection<UserDTO> friendList;
        public ConversationViewModel(FriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
            RegisterService();
        }
        private void RegisterService()
        {
            WeakReferenceMessenger.Default.Register<RefreshFriendListRequest>(this, async (r, action) => { await RefreshFriendList(action.Token); });
        }
        private async Task RefreshFriendList(string token)
        {
            try
            {
                var result = await _friendshipService.GetFriendsAsync(token);
                if (result == null) { return; }
                FriendList = new ObservableCollection<UserDTO>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
