using LOKI_Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class AttachmentDTO
    {
        public Guid AttachmentId { get; set; } // Primary Key
        public Guid MessageId { get; set; } // Foreign Key
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public FileType FileType { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
