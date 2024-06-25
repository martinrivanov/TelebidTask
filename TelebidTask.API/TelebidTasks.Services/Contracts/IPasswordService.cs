using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelebidTask.Services.Contracts
{
    public interface IPasswordService
    {
        byte[] GenerateSalt();
        string GeneratePasswordHash(string password, byte[] salt);
    }
}
