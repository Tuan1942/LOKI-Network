using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.Models;
using LOKI_Client.UIs.ViewModels.Account;
using LOKI_Client.UIs.ViewModels.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LOKI_Client.UIs.Views.Message
{
    /// <summary>
    /// Interaction logic for DirectMessagePage.xaml
    /// </summary>
    public partial class DirectMessagePage : Page
    {
        public DirectMessagePage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MessageViewModel));
            RegisterServices();
        }

        private void RegisterServices()
        {
            WeakReferenceMessenger.Default.Register<ScrollToBottomRequest>(this, (r, m) =>
            {
                ScrollToBottom();
            });

        }
        private void ScrollToBottom()
        {
            if (MainContent != null)
            {
                MainContent.ScrollToEnd();
            }
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ScrollViewer mainContent)
                ScrollToBottom();

        }

    }
}
