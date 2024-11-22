using LOKI_Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Models
{
    public class FriendshipDTO
    {
        public Guid FriendshipId { get; set; } // Primary Key
        public Guid UserId { get; set; } // Foreign Key
        public Guid FriendId { get; set; } // Foreign Key
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public UserDTO User { get; set; }
        public UserDTO Friend { get; set; }
    }
}
