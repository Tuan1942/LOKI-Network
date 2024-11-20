using Microsoft.EntityFrameworkCore;

namespace LOKI_Network.DbContexts
{
    public class LokiContext : DbContext
    {
        public LokiContext(DbContextOptions<LokiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.ConversationParticipants)
                .HasForeignKey(cp => cp.ConversationId);

            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.ConversationParticipants)
                .HasForeignKey(cp => cp.UserId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<MessageStatus>()
                .HasOne(ms => ms.Message)
                .WithMany(m => m.MessageStatuses)
                .HasForeignKey(ms => ms.MessageId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MessageStatus>()
                .HasOne(ms => ms.User)
                .WithMany()
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<UserSettings>()
                .HasOne(us => us.User)
                .WithOne(u => u.UserSettings)
                .HasForeignKey<UserSettings>(us => us.UserId);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MessageId);

            modelBuilder.Entity<ModifyHistory>()
                .HasOne(mh => mh.User)
                .WithMany()
                .HasForeignKey(mh => mh.UserId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ModifyHistory> ModifyHistories { get; set; }
    }

    public class User
    {
        public Guid UserId { get; set; } // Primary Key
        public string Username { get; set; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }

        // Navigation properties
        public ICollection<Friendship> Friendships { get; set; }
        public ICollection<ConversationParticipant> ConversationParticipants { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public UserSettings UserSettings { get; set; }
    }

    public class Friendship
    {
        public Guid FriendshipId { get; set; } // Primary Key
        public Guid UserId { get; set; } // Foreign Key
        public Guid FriendId { get; set; } // Foreign Key
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public User Friend { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Blocked
    }

    public class Conversation
    {
        public Guid ConversationId { get; set; } // Primary Key
        public bool IsGroup { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public ICollection<ConversationParticipant> ConversationParticipants { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

    public class ConversationParticipant
    {
        public Guid ConversationParticipantId { get; set; } // Primary Key
        public Guid ConversationId { get; set; } // Foreign Key
        public Guid UserId { get; set; } // Foreign Key
        public DateTime JoinedDate { get; set; }

        // Navigation properties
        public Conversation Conversation { get; set; }
        public User User { get; set; }
    }

    public class Message
    {
        public Guid MessageId { get; set; } // Primary Key
        public Guid ConversationId { get; set; } // Foreign Key
        public Guid SenderId { get; set; } // Foreign Key
        public string Content { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }

        // Navigation properties
        public ICollection<Attachment> Attachments { get; set; }
        public Conversation Conversation { get; set; }
        public User Sender { get; set; }
        public ICollection<MessageStatus> MessageStatuses { get; set; }
    }

    public enum MessageType
    {
        Text,
        Media,
        File
    }
    public class MessageStatus
    {
        public Guid MessageStatusId { get; set; } // Primary Key
        public Guid MessageId { get; set; } // Foreign Key
        public Guid UserId { get; set; } // Foreign Key
        public bool IsRead { get; set; }
        public DateTime ReadDate { get; set; }

        // Navigation properties
        public Message Message { get; set; }
        public User User { get; set; }
    }

    public class Notification
    {
        public Guid NotificationId { get; set; } // Primary Key
        public Guid UserId { get; set; } // Foreign Key
        public NotificationType Type { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
    }

    public enum NotificationType
    {
        Message,
        FriendRequest,
        Mention
    }

    public class UserSettings
    {
        public Guid UserSettingsId { get; set; } // Primary Key
        public Guid UserId { get; set; } // Foreign Key
        public bool IsOnline { get; set; }
        public bool IsNotificationsEnabled { get; set; }
        public Theme Theme { get; set; }

        // Navigation properties
        public User User { get; set; }
    }

    public enum Theme
    {
        Light,
        Dark
    }
    public class Attachment
    {
        public Guid AttachmentId { get; set; } // Primary Key
        public Guid MessageId { get; set; } // Foreign Key
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public FileType FileType { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public Message Message { get; set; }
    }

    public enum FileType
    {
        Image,
        Video,
        Audio,
        Document,
        Other
    }
    public class ModifyHistory
    {
        public Guid ModifyHistoryId { get; set; } // Primary Key
        public Guid UserId { get; set; } // Foreign Key
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ModifiedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
    }

    public enum EntityType
    {
        Message,
        Profile
    }

}
