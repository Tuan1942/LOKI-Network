using LOKI_Model.Enums;
using LOKI_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LOKI_Client.Models.Objects
{
    public class AttachmentObject : AttachmentDTO
    {
        public AttachmentObject(AttachmentDTO dto)
        {
            AttachmentId = dto.AttachmentId;
            MessageId = dto.MessageId;
            FileName = dto.FileName;
            FileUrl = dto.FileUrl;
            FileType = dto.FileType;
            CreatedDate = dto.CreatedDate;
        }
        public BitmapImage FileImage { get; set; }

        public bool IsLoaded { get; set; }
    }
}
