using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
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
        private readonly IConversationService _conversationService;

        [ObservableProperty]
        private ObservableCollection<ConversationDTO> conversations;
        public RelayCommand<ConversationDTO> OpenConversationCommand => new RelayCommand<ConversationDTO>(OpenConversation);
        public ConversationViewModel(FriendshipService friendshipService, IConversationService conversationService)
        {
            _friendshipService = friendshipService;
            _conversationService = conversationService;
            RegisterService();
        }
        private void RegisterService()
        {
            WeakReferenceMessenger.Default.Register<RefreshConversationListRequest>(this, async (r, action) => { await RefreshConversationListAsync(action.Token); });
        }
        private async Task RefreshConversationListAsync(string token)
        {
            try
            {
                var result = await _conversationService.GetConversationsAsync();
                if (result == null) { return; }
                Conversations = new ObservableCollection<ConversationDTO>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void OpenConversation(ConversationDTO conversation)
        {
            WeakReferenceMessenger.Default.Send(new RefreshConversationMessages(conversation));
        }
    }
}
