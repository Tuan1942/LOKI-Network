using LOKI_Client.Models.Helper;
using LOKI_Client.Models.Objects;
using System.Text.Json;

namespace LOKI_Client.Extensions.Authorize
{
    public class UserProvider
    {
        private UserObject _user;

        public UserProvider()
        {
            var userJson = ApplicationSettingsHelper.GetSetting(nameof(_user));
            if (!string.IsNullOrEmpty(userJson))
            {
                _user = JsonSerializer.Deserialize<UserObject>(userJson) ?? new UserObject();
            }
            else _user = new UserObject();
        }
        public UserObject User
        {
            get { return _user; }
            set { 
                _user = value; 
                var content = JsonSerializer.Serialize(_user);
                ApplicationSettingsHelper.SaveSetting(nameof(_user), content);
            }
        }
        public string GetToken() => _user?.Token;

        public bool IsTokenExpired() => DateTime.UtcNow >= _user?.TokenExpirationDate;
    }
}
