using LOKI_Model.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class MessageDTO
    {
        public Guid MessageId { get; set; } // Primary Key
        public Guid ConversationId { get; set; } // Foreign Key
        public Guid SenderId { get; set; } // Foreign Key
        public string Content { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }

        // Navigation properties
        public ICollection<Attachment> Attachments { get; set; }
    }
}
