using CommunityToolkit.Mvvm.Messaging.Messages;
using LOKI_Client.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LOKI_Client.Models
{
    public class LoginRequest : ValueChangedMessage<User>
    {
        public LoginRequest(User user) : base(user)
        {
            User = user;
        }

        public User User { get; set; }
    }
    public class OpenLoginPageRequest
    {

    }
    public class ConnectWebSocketRequest : ValueChangedMessage<User>
    {
        public ConnectWebSocketRequest(User user) : base(user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}
