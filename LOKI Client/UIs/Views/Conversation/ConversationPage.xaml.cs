using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LOKI_Client.Models;
using LOKI_Client.UIs.ViewModels.Account;
using LOKI_Client.UIs.ViewModels.Conversation;
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

namespace LOKI_Client.UIs.Views.Conversation
{
    /// <summary>
    /// Interaction logic for ConversationPage.xaml
    /// </summary>
    public partial class ConversationPage : Page
    {
        public ConversationPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(ConversationViewModel));
        }
    }
}
