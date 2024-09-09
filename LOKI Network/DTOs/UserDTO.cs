using System.ComponentModel.DataAnnotations;

namespace LOKI_Network.DTOs
{
    public class UserDTO
    {
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool Gender { get; set; }

        public UserDTO() { }
    }
}
