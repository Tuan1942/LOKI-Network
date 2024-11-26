using LOKI_Client.Models.Helper;
using LOKI_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LOKI_Client.Extensions.Authorize
{
    public class UserProvider
    {
        private UserDTO _user;
        private string _token;

        public UserProvider()
        {
            var userJson = ApplicationSettingsHelper.GetSetting(nameof(_user));
            if (!string.IsNullOrEmpty(userJson))
            {
                _user = JsonSerializer.Deserialize<UserDTO>(userJson) ?? new UserDTO();
            }
            else _user = new UserDTO();
        }
        public UserDTO User
        {
            get { return _user; }
            set { 
                _user = value; 
                var content = JsonSerializer.Serialize(_user);
                ApplicationSettingsHelper.SaveSetting(nameof(_user), content);
            }
        }
        public string GetToken() => _user?.Token;

    }
}
