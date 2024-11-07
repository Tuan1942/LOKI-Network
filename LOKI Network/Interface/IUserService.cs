using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;

namespace LOKI_Network.Interface
{
    public interface IUserService
    {
        Task AddUser(UserDTO user);
        Task RemoveUser(Guid userId);
        Task UpdateUser(UserDTO user);
        Task<User> GetUser(Guid id);
        Task<User> GetUser(string username);
        bool VerifyPassword(string enteredPassword, string storedHash);
        string GenerateJwtToken(User user, IConfiguration configuration);
    }
}
