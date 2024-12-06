using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.Models;
using LOKI_Client.Models.Objects;
using System.Collections.ObjectModel;

namespace LOKI_Client.UIs.ViewModels.Conversation
{
    public partial class ConversationViewModel : ObservableObject
    {
        private readonly FriendshipService _friendshipService;
        private readonly IConversationService _conversationService;

        [ObservableProperty]
        private ObservableCollection<ConversationObject> conversations;

        [ObservableProperty]
        private ObservableCollection<ConversationObject> filteredConversations;

        [ObservableProperty]
        private string searchText;
        public IRelayCommand ClearSearchCommand => new RelayCommand(() => SearchText = string.Empty);

        public RelayCommand<ConversationObject> OpenConversationCommand => new RelayCommand<ConversationObject>(OpenConversation);
        public ConversationViewModel(FriendshipService friendshipService, IConversationService conversationService)
        {
            _friendshipService = friendshipService;
            _conversationService = conversationService;
            RegisterService();
        }
        private void RegisterService()
        {
            WeakReferenceMessenger.Default.Register<RefreshConversationListRequest>(this, async (r, action) => { await RefreshConversationListAsync(); });
            WeakReferenceMessenger.Default.Register<AddMessageRequest>(this, (r, action) => NotifyConversation(action.Message));
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SearchText))
                {
                    PerformSearch();
                }
            };
        }
        private void PerformSearch()
        {
            if (Conversations == null) return;
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredConversations = new ObservableCollection<ConversationObject>(Conversations);
            }
            else
            {
                var lowerSearchText = SearchText.ToLower();
                var filtered = Conversations
                    .Where(c => c.Name.ToLower().Contains(lowerSearchText))
                    .ToList();

                FilteredConversations = new ObservableCollection<ConversationObject>(filtered);
            }
        }
        private async Task RefreshConversationListAsync()
        {
            try
            {
                var result = await _conversationService.GetConversationsAsync();
                if (result == null) { return; }
                Conversations = new ObservableCollection<ConversationObject>(result);
                FilteredConversations = new ObservableCollection<ConversationObject>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void OpenConversation(ConversationObject conversation)
        {
            WeakReferenceMessenger.Default.Send(new RefreshConversationMessages(conversation));
        }
        private void NotifyConversation(MessageObject message)
        {
            var conversation = Conversations.FirstOrDefault(c => c.ConversationId == message.ConversationId);
            if (conversation == null) return;

            // Check if already at the top
            if (Conversations.IndexOf(conversation) == 0) return;

            // Reorder
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                Conversations.Remove(conversation);
                Conversations.Insert(0, conversation);
            });
        }
    }
}
