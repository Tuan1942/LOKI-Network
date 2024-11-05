using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;

namespace LOKI_Network.Interface
{
    public interface IUserService
    {
        void AddUser(UserDTO user);
        void RemoveUser(Guid userId);
        void UpdateUser(UserDTO user);
        User GetUser(Guid id);
        User GetUser(string username);
        bool VerifyPassword(string enteredPassword, string storedHash);
        string GenerateJwtToken(User user, IConfiguration configuration);
    }
}
