using LOKI_Model.Models;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace LOKI_Client.Models.Objects
{
    public class AttachmentObject : AttachmentDTO, INotifyPropertyChanged
    {
        public AttachmentObject() { }
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
