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
        User GetUserById(Guid Id);
        User Login(LoginCredentials credentials);
        Task<User> Register(UserDTO userDTO);
        User UpdateUser(Guid id, JsonPatchDocument<User> patch);
    }
}
