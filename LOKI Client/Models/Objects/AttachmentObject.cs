using LOKI_Model.Models;
using System.ComponentModel;
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
        private BitmapImage fileImage { get; set; }
        public BitmapImage FileImage
        {
            get => fileImage;
            set
            {
                fileImage = value;
                OnPropertyChanged(nameof(FileImage));
            }
        }

        public bool isLoaded { get; set; }
        public bool IsLoaded
        {
            get => isLoaded;
            set
            {
                isLoaded = value;
                OnPropertyChanged(nameof(IsLoaded));
            }
        }
    }
}
