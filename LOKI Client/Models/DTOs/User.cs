using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Models.DTOs
{
    public class User
    {
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Token { get; set; }
        public bool Gender { get; set; }
    }
}
