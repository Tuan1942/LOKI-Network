using LOKI_Model.Models;
using System.ComponentModel;

namespace LOKI_Client.Models.Objects
{
    public class ConversationObject : ConversationDTO, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
