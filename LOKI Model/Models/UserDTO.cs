using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class UserDTO : INotifyPropertyChanged
    {
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Token { get; set; }
        public bool Gender { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
