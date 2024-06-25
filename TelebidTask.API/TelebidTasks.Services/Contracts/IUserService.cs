using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelebidTask.Data.Models;

namespace TelebidTask.Services.Contracts
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(Guid Id);
        Task<UserDTO> Login(LoginCredentials credentials);
        Task<UserDTO> Register(RegistrationUserModel userDTO);
        Task<UserDTO> UpdateUser(Guid id, JsonPatchDocument<User> patch);
    }
}
