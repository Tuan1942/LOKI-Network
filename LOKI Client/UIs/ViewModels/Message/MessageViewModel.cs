using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.Models;
using LOKI_Client.Models.Helper;
using LOKI_Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        public bool IsFilesSelected => SelectedFiles?.Any() ?? false;

        private ObservableCollection<FileMetadata> selectedFiles = new ObservableCollection<FileMetadata>();

        public ObservableCollection<FileMetadata> SelectedFiles
        {
            get => selectedFiles;
            set
            {
                if (selectedFiles != null)
                {
                    selectedFiles.CollectionChanged -= SelectedFiles_CollectionChanged;
                }

                SetProperty(ref selectedFiles, value);

                if (selectedFiles != null)
                {
                    selectedFiles.CollectionChanged += SelectedFiles_CollectionChanged;
                }
                OnPropertyChanged(nameof(IsFilesSelected));
                OnPropertyChanged(nameof(SelectedFiles));
            }
        }
        private void SelectedFiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsFilesSelected));
        }

        public RelayCommand SelectFilesCommand => new RelayCommand(SelectFiles);
        public RelayCommand ClearSelectedFilesCommand => new RelayCommand(ClearSelectedFiles);
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
                App.Current.Dispatcher.Invoke(() =>
                {
                    Messages = new ObservableCollection<MessageDTO>(messageList);
                });

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
                await _conversationService.SendMessageAsync(Conversation.ConversationId, message, SelectedFiles?.Select(f => f.File).ToList());
                InputContent = string.Empty;
                SelectedFiles = null;
            }
            catch (Exception ex) 
            { }
        }

        private async Task AddMessage(MessageDTO message)
        {
            try
            {
                if (message.ConversationId == Conversation.ConversationId)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messages.Add(message);
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        private void SelectFiles()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All Files|*.*",
                Title = "Select Files to Attach"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedFiles = new ObservableCollection<FileMetadata>(
                    dialog.FileNames.Select(filePath =>
                    {
                        var fileInfo = new FileInfo(filePath);

                        return new FileMetadata
                        {
                            File = new FormFile(new FileStream(filePath, FileMode.Open, FileAccess.Read), 0, fileInfo.Length, "file", fileInfo.Name)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = FileHelper.GetMimeType(fileInfo.Extension)
                            },
                            Preview = GenerateFilePreview(filePath) // Generate a preview for each file
                        };
                    }));
            }
        }

        private void ClearSelectedFiles()
        {
            if (SelectedFiles != null) 
                SelectedFiles.Clear();
        }

        private BitmapImage GenerateFilePreview(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            // For image files
            if (extension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".jfif")
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            // For video files
            else if (extension is ".mp4" or ".mkv" or ".avi" or ".mov" or ".wmv")
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/video_icon.png"));
            }
            // For audio files
            else if (extension is ".mp3" or ".wav" or ".ogg" or ".aac" or ".flac")
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/audio_icon.png"));
            }
            // For word files
            else if (extension is ".doc" or ".docx")
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/doc_icon.png"));
            }
            // For excel files
            else if (extension is ".xlsx" or ".xls")
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/xls_icon.png"));
            }
            // For power point files
            else if (extension is ".ppt" or ".pptx")
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/ppt_icon.png"));
            }
            // For pdf files
            else if (extension is ".pdf")
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/pdf_icon.png"));
            }
            // Default for other file types
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/FileIcons/oth_icon.png"));
            }
        }
    }
    public class FileMetadata
    {
        public IFormFile File { get; set; }
        public BitmapImage Preview { get; set; } // Preview image for display
    }
}
