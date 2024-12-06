using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class UserDTO
    {
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string FullName
        {
            get
            {
                var firstName = string.IsNullOrEmpty(FirstName) ? "" : FirstName + " ";
                var middleName = string.IsNullOrEmpty(MiddleName) ? "" : MiddleName + " ";
                var lastName = string.IsNullOrEmpty(LastName) ? "" : LastName + " ";
                return (firstName + middleName + lastName).Trim();
            }
        }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool Gender { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }
}
