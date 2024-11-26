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
using System.Windows.Controls;

namespace LOKI_Client.UIs.ViewModels.Message
{
    public partial class MessageViewModel : ObservableObject
    {
        private readonly IConversationService _conversationService;
        [ObservableProperty]
        ObservableCollection<MessageDTO> messages;

        [ObservableProperty]
        private ConversationDTO conversation;

        [ObservableProperty]
        private string inputContent;

        [ObservableProperty]
        private bool isLoadingMessages;

        public RelayCommand SendMessageCommand => new RelayCommand(async () => await SendMessage());
        public RelayCommand<ScrollViewer> LoadNextMessagesCommand => new RelayCommand<ScrollViewer>(async (s) => await CheckForLoadNextMessages(s));

        public MessageViewModel(IConversationService conversationService)
        {
            _conversationService = conversationService;
            RegisterServices();
        }

        private async void RegisterServices()
        {
            WeakReferenceMessenger.Default.Register<RefreshConversationMessages>(this, async (r, action) => await RefreshMessages(action.Conversation));
            WeakReferenceMessenger.Default.Register<AddMessageRequest>(this, async (r, action) => await AddMessage(action.Message));
        }

        async Task RefreshMessages(ConversationDTO conversation)
        {
            try
            {
                Conversation = conversation;
                var messageList = await _conversationService.GetMessagesByConversationAsync(Conversation.ConversationId, 1);
                Messages = new ObservableCollection<MessageDTO>(messageList);
                WeakReferenceMessenger.Default.Send(new ScrollToBottomRequest());
            }
            catch (Exception ex)
            {
            }
        }
        private async Task CheckForLoadNextMessages(ScrollViewer scrollViewer)
        {
            if (scrollViewer == null || IsLoadingMessages) return;
            if (scrollViewer.VerticalOffset == 0)
            {
                await GetNextMessages(scrollViewer);
            }
        }
        private async Task GetNextMessages(ScrollViewer scrollViewer)
        {
            if (IsLoadingMessages) return; // Avoid concurrent loads

            IsLoadingMessages = true;

            try
            {
                var lastMessage = Messages.FirstOrDefault();
                if (lastMessage == null) return;

                // Capture the current scroll position and extent height
                double previousOffset = scrollViewer.VerticalOffset;
                double previousExtentHeight = scrollViewer.ExtentHeight;

                // Fetch messages and insert them at the top
                var messageList = await _conversationService.GetNextMessagesAsync(Conversation.ConversationId, lastMessage.MessageId);

                foreach (var message in messageList)
                {
                    Messages.Insert(0, message);
                }

                await Task.Delay(1);
                await scrollViewer.Dispatcher.InvokeAsync(() =>
                {
                    // Restore the scroll position
                    double newExtentHeight = scrollViewer.ExtentHeight;
                    double heightDifference = newExtentHeight - previousExtentHeight;
                    scrollViewer.ScrollToVerticalOffset(previousOffset + heightDifference);
                });
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsLoadingMessages = false;
            }
        }

        private async Task SendMessage()
        {
            try
            {
                var message = new MessageDTO
                {
                    ConversationId = Conversation.ConversationId,
                    Content = InputContent,
                };
                InputContent = string.Empty;
                await _conversationService.SendMessageAsync(Conversation.ConversationId, message);
            }
            catch (Exception ex) { }
        }

        private async Task AddMessage(MessageDTO message)
        {
            if (message.ConversationId == Conversation.ConversationId)
            {
                Messages.Add(message);
            }
        }
    }
}
