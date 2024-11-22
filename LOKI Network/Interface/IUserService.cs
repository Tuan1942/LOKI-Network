using LOKI_Model.Models;

namespace LOKI_Network.Interface
{
    public interface IUserService
    {
        Task AddUser(UserDTO user);
        Task RemoveUser(Guid userId);
        Task UpdateUser(UserDTO user);
        Task<UserDTO> GetUser(Guid id);
        Task<UserDTO> GetUser(string username);
        bool VerifyPassword(UserDTO user);
        //string GenerateJwtToken(UserDTO user, IConfiguration configuration);
        //Task<UserDTO> ValidateJwtToken(string token, IConfiguration configuration);
    }
}
