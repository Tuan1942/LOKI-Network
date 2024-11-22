using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Model.Enums
{
    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Blocked
    }
    public enum MessageType
    {
        Text,
        Media,
        File
    }
    public enum NotificationType
    {
        Message,
        FriendRequest,
        Mention
    }
    public enum Theme
    {
        Light,
        Dark
    }
    public enum FileType
    {
        Image,
        Video,
        Audio,
        Document,
        Other
    }
    public enum EntityType
    {
        Message,
        Profile
    }
    public enum ParticipantRole
    {
        None,
        Vice,
        Admin
    }
}
