using LOKI_Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.ApiClients.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> Login(UserDTO user);
        Task<bool> Register(UserDTO user);
    }
}
