using LOKI_Model.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LOKI_Client.Models.Objects
{
    public class MessageObject : MessageDTO, INotifyPropertyChanged
    {
        public MessageObject() 
        {
        }
        public MessageObject(MessageDTO dto)
        {
            MessageId = dto.MessageId;
            ConversationId = dto.ConversationId;
            SenderId = dto.SenderId;
            Content = dto.Content;
            SentDate = dto.SentDate;
            IsRead = dto.IsRead;
            User = dto.User;
            var attachments = dto.Attachments?.Select(a => new AttachmentObject(a)).ToList() ?? new List<AttachmentObject>();
            Attachments = new ObservableCollection<AttachmentObject>(attachments);
        }
        public new ObservableCollection<AttachmentObject> Attachments { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
