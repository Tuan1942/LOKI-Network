using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.Models;
using LOKI_Client.Models.Helper;
using LOKI_Client.Models.Objects;
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
        private int maxInputFiles = 20;
        [ObservableProperty]
        ObservableCollection<MessageObject> messages;

        [ObservableProperty]
        private ConversationObject conversation;

        [ObservableProperty]
        private string inputContent;

        [ObservableProperty]
        private bool isLoadingMessages;

        [ObservableProperty]
        private ObservableCollection<FileMetadata> selectedFiles;

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
            WeakReferenceMessenger.Default.Register<RefreshConversationMessages>(this, async (r, action) => await ChangeConversation(action.Conversation));
            WeakReferenceMessenger.Default.Register<AddMessageRequest>(this, async (r, action) => await AddMessage(action.Message));
        }

        async Task ChangeConversation(ConversationObject conversation)
        {
            try
            {
                if (Conversation == null) Conversation = new();
                if (conversation.ConversationId == Conversation.ConversationId) return;
                Conversation = conversation;

                var messageList = await _conversationService.GetMessagesByConversationAsync(Conversation.ConversationId, 1);

                if (messageList.Any()) 
                    Messages = new ObservableCollection<MessageObject>(messageList);
                else 
                    Messages = new ObservableCollection<MessageObject>();

                WeakReferenceMessenger.Default.Send(new ScrollToBottomRequest());

                // Load attachments in the background
                await LoadAttachmentsAsync(Messages);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }
        private async Task LoadAttachmentsAsync(IEnumerable<MessageObject> messages)
        {
            const int batchSize = 5; // Number of attachments to load at a time
            var attachmentsToLoad = messages
                .OrderByDescending(m => m.SentDate)
                .SelectMany(m => m.Attachments)
                .Where(a => a.FileImage == null)
                .ToList();

            for (int i = 0; i < attachmentsToLoad.Count; i += batchSize)
            {
                var batch = attachmentsToLoad.Skip(i).Take(batchSize).ToList();

                // Process each batch
                foreach (var attachment in batch)
                {
                    try
                    {
                        if (attachment.FileImage != null) return;
                        BitmapImage bitmap = null;

                        // Create and initialize the BitmapImage on the UI thread
                        await App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(attachment.FileUrl, UriKind.Absolute);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();

                            // Update attachment properties
                            attachment.FileImage = bitmap;
                        });

                        // Add a slight delay to smoothen the UI update
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
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
                    await AddMessage(message, true);
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
                var message = new MessageObject
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

        private async Task AddMessage(MessageObject message, bool isPrepend = false)
        {
            try
            {
                if (message.ConversationId == Conversation.ConversationId)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (isPrepend)
                            Messages.Insert(0, message);
                        else
                            Messages.Add(message);
                    });
                    foreach (var attachment in message.Attachments)
                    {
                        try
                        {
                            if (attachment.FileImage != null) continue;
                            BitmapImage bitmap = null;

                            // Create and initialize the BitmapImage on the UI thread
                            await App.Current.Dispatcher.InvokeAsync(() =>
                            {
                                bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(attachment.FileUrl, UriKind.Absolute);
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();

                                // Update attachment properties
                                attachment.FileImage = bitmap;
                            });

                            // Add a slight delay to smoothen the UI update
                            await Task.Delay(100);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void SelectFiles()
        {
            var remainingFiles = maxInputFiles - SelectedFiles?.Count();
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All Files|*.*",
                Title = $"Select Files to Attach (Max: {remainingFiles} Remaining)"
            };

            if (dialog.ShowDialog() == true)
            {
                // Limit the selected files to 20
                var selectedFilePaths = dialog.FileNames.Take(maxInputFiles).ToList();
                if (dialog.FileNames.Length > maxInputFiles)
                {
                    MessageBox.Show($"You can only attach up to {maxInputFiles} files.", "File Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (SelectedFiles == null) SelectedFiles = new ObservableCollection<FileMetadata>();

                var fileSelected = (
                    selectedFilePaths.Select(filePath =>
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

                foreach (var file in fileSelected)
                {
                    SelectedFiles.Add(file);
                }
            }
        }

        private void ClearSelectedFiles()
        {
            if (SelectedFiles != null) 
                SelectedFiles = null;
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
}
