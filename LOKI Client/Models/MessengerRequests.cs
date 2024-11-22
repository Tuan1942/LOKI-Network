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
    public class RefreshFriendListRequest : ValueChangedMessage<string>
    {
        public RefreshFriendListRequest(string token) : base(token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
    public class OpenLoginPageRequest
    {

    }
    public class ConnectWebSocketRequest : ValueChangedMessage<UserDTO>
    {
        public ConnectWebSocketRequest(UserDTO user) : base(user)
        {
            User = user;
        }

        public UserDTO User { get; set; }
    }
}
