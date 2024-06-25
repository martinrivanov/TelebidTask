using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelebidTask.Data.Models;

namespace TelebidTask.Data.Contracts
{
    public interface IDatabaseRepository
    {
        Task<User> GetUserById(Guid id);
        Task<User> GetUserByEmail(string email);
        Task<User> CreateUser(User registrationModel);
        Task UpdateUser(Guid id, User user);
    }
}
