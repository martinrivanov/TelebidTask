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
        IEnumerable<User> GetUsers();
        User GetUserById(Guid id);
        User GetUserByEmail(string email);
        bool IsThereAUserWithEmail(string email);
        User CreateUser(User registrationModel);
        void UpdateUser(Guid id, User user);
    }
}
