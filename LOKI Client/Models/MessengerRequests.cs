using CommunityToolkit.Mvvm.Messaging.Messages;
using LOKI_Client.Models.Objects;

namespace LOKI_Client.Models
{
    public class LoginRequest : ValueChangedMessage<UserObject>
    {
        public LoginRequest(UserObject user) : base(user)
        {
            User = user;
        }

        public UserObject User { get; set; }
    }
    public class OpenProfilePageRequest : ValueChangedMessage<UserObject>
    {
        public OpenProfilePageRequest(UserObject user) : base(user)
        {
            User = user;
        }

        public UserObject User { get; set; }
    }
    public class UpdateProfilePageRequest : ValueChangedMessage<UserObject>
    {
        public UpdateProfilePageRequest(UserObject user) : base(user)
        {
            User = user;
        }

        public UserObject User { get; set; }
    }
    public class RefreshConversationListRequest
    {
        //public RefreshConversationListRequest(string token) : base(token)
        //{
        //    Token = token;
        //}

        //public string Token { get; set; }
    }
    public class AddMessageRequest : ValueChangedMessage<MessageObject>
    {
        public AddMessageRequest(MessageObject message) : base(message)
        {
            Message = message;
        }

        public MessageObject Message { get; set; }
    }
    public class RefreshConversationMessages : ValueChangedMessage<ConversationObject>
    {
        public RefreshConversationMessages(ConversationObject conversation) : base(conversation)
        {
            Conversation = conversation;
        }

        public ConversationObject Conversation { get; set; }
    }
    public class OpenLoginPageRequest
    {

    }
    public class ScrollToBottomRequest
    {

    }
    public class ConnectSignalRRequest
    {
        //public ConnectSignalRRequest(string token) : base(token)
        //{
        //    Token = token;
        //}

        //public string Token { get; set; }
    }
}
