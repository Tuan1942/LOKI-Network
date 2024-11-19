using LOKI_Client.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.ApiClients.Interfaces
{
    public interface IUserService
    {
        Task<string> Login(User user);
        Task<bool> Register(User user);
    }
}
