using LOKI_Client.Models.Objects;

namespace LOKI_Client.ApiClients.Interfaces
{
    public interface IUserService
    {
        Task<UserObject> Login(UserObject user);
        Task<bool> Register(UserObject user);
    }
}
