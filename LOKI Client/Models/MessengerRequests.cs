using CommunityToolkit.Mvvm.Messaging.Messages;
using LOKI_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LOKI_Client.Models
{
    public class LoginRequest : ValueChangedMessage<UserDTO>
    {
        public LoginRequest(UserDTO user) : base(user)
        {
            User = user;
        }

        public UserDTO User { get; set; }
    }
    public class RefreshConversationListRequest : ValueChangedMessage<string>
    {
        public RefreshConversationListRequest(string token) : base(token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
    public class AddMessageRequest : ValueChangedMessage<MessageDTO>
    {
        public AddMessageRequest(MessageDTO message) : base(message)
        {
            Message = message;
        }

        public MessageDTO Message { get; set; }
    }
    public class RefreshConversationMessages : ValueChangedMessage<ConversationDTO>
    {
        public RefreshConversationMessages(ConversationDTO conversation) : base(conversation)
        {
            Conversation = conversation;
        }

        public ConversationDTO Conversation { get; set; }
    }
    public class OpenLoginPageRequest
    {

    }
    public class ScrollToBottomRequest
    {

    }
    public class ConnectWebSocketRequest : ValueChangedMessage<string>
    {
        public ConnectWebSocketRequest(string token) : base(token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
