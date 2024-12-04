using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class ConversationDTO : INotifyPropertyChanged
    {
        public Guid ConversationId { get; set; }
        public string? Name { get; set; }
        public bool IsGroup { get; set; }
        public ObservableCollection<UserDTO> Users { get; set; }
        public DateTime CreatedDate { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
