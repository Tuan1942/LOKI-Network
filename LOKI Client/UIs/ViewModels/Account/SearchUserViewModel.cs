using CommunityToolkit.Mvvm.ComponentModel;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.Models.Objects;
using System.Collections.ObjectModel;

namespace LOKI_Client.UIs.ViewModels.Account
{
    public partial class SearchUserViewModel : ObservableObject
    {
        private readonly FriendshipService _riendshipService;
        public SearchUserViewModel(FriendshipService friendshipService)
        {
            _riendshipService = friendshipService;
        }

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private ObservableCollection<UserObject> filteredUsers;

        //private void PerformSearch()
        //{
        //    if (string.IsNullOrWhiteSpace(SearchText))
        //    {
        //        FilteredUsers = null;
        //    }
        //    else
        //    {
        //        var users = new ObservableCollection<UserDTO>();
        //        var lowerSearchText = SearchText.ToLower();
        //        var filtered = Conversations
        //            .Where(c => c.Name.ToLower().Contains(lowerSearchText))
        //            .ToList();

        //        FilteredConversations = new ObservableCollection<ConversationDTO>(filtered);
        //    }
        //}


        //private void PerformSearch()
        //{
        //    if (string.IsNullOrWhiteSpace(SearchText))
        //    {
        //        FilteredConversations = new ObservableCollection<ConversationDTO>(Conversations);
        //    }
        //    else
        //    {
        //        var lowerSearchText = SearchText.ToLower();
        //        var filtered = Conversations
        //            .Where(c => c.Name.ToLower().Contains(lowerSearchText))
        //            .ToList();

        //        FilteredConversations = new ObservableCollection<ConversationDTO>(filtered);
        //    }
        //}

    }
}
