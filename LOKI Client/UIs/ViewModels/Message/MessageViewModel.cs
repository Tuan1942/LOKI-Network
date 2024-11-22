using CommunityToolkit.Mvvm.ComponentModel;
using LOKI_Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.UIs.ViewModels.Message
{
    public partial class MessageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<MessageDTO> messages;
        public MessageViewModel()
        {
            Messages = new ObservableCollection<MessageDTO>()
            {
new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Hello, how are you?", SentDate = DateTime.Now.AddMinutes(-10) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "I'm good, thanks!", SentDate = DateTime.Now.AddMinutes(-9) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "What are you up to?", SentDate = DateTime.Now.AddMinutes(-8) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Just working on a project.", SentDate = DateTime.Now.AddMinutes(-7) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Sounds interesting!", SentDate = DateTime.Now.AddMinutes(-6) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Yeah, it's quite challenging.", SentDate = DateTime.Now.AddMinutes(-5) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Need any help?", SentDate = DateTime.Now.AddMinutes(-4) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "That would be great, thanks!", SentDate = DateTime.Now.AddMinutes(-3) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "No problem, what do you need?", SentDate = DateTime.Now.AddMinutes(-2) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Can you review my code?", SentDate = DateTime.Now.AddMinutes(-1) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Sure, send it over.", SentDate = DateTime.Now }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Here it is.", SentDate = DateTime.Now.AddSeconds(-50) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Got it, let me take a look.", SentDate = DateTime.Now.AddSeconds(-40) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Thanks!", SentDate = DateTime.Now.AddSeconds(-30) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "No worries.", SentDate = DateTime.Now.AddSeconds(-20) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "How's it looking?", SentDate = DateTime.Now.AddSeconds(-10) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Looks good, just a few tweaks needed.", SentDate = DateTime.Now }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Great, I'll fix them.", SentDate = DateTime.Now.AddSeconds(10) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Let me know if you need anything else.", SentDate = DateTime.Now.AddSeconds(20) }, new MessageDTO { MessageId = Guid.NewGuid(), SenderId = Guid.NewGuid(), Content = "Will do, thanks again!", SentDate = DateTime.Now.AddSeconds(30) }            };
        }
    }
}
