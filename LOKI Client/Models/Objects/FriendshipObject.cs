using LOKI_Model.Models;
using System.ComponentModel;

namespace LOKI_Client.Models.Objects
{
    public class FriendshipObject : FriendshipDTO
    {
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
