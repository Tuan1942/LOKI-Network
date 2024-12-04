using LOKI_Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Models.Objects
{
    public class MessageObject : MessageDTO
    {
        public MessageObject(MessageDTO dto)
        {
            MessageId = dto.MessageId;
            ConversationId = dto.ConversationId;
            SenderId = dto.SenderId;
            Content = dto.Content;
            SentDate = dto.SentDate;
            IsRead = dto.IsRead;
            User = dto.User;
            Attachments = new ObservableCollection<AttachmentObject>(dto.Attachments.Select(a => new AttachmentObject(a)));
        }
        public new ObservableCollection<AttachmentObject> Attachments { get; set; }
    }
}
