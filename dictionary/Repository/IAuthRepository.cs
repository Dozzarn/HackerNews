using dictionary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary.Repository
{
    public interface IAuthRepository : ICrud<UserDTO>
    {
        Task<UserDTO> Register(UserDTO user, string password);
        Task<UserDTO> Login(string userName, string password);
        Task<bool> UserExits(string userName);
    }
}
