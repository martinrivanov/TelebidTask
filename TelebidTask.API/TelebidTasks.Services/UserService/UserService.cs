using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelebidTask.Data.Contracts;
using TelebidTask.Data.Models;
using TelebidTask.Services.Contracts;

namespace TelebidTask.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IDatabaseRepository repository;
        private readonly IPasswordService passwordService;

        public UserService(IDatabaseRepository repository, IPasswordService passwordService)
        {
            this.repository = repository;
            this.passwordService = passwordService;
        }

        public User GetUserById(Guid Id)
        {
            return repository.GetUserById(Id);
        }

        public User Login(LoginCredentials credentials)
        {
            var user = repository.GetUserByEmail(credentials.Email);

            if (user == null)
                return null;

            var salt = Convert.FromBase64String(user.Salt);
            var enteredPasswordHash = passwordService.GeneratePasswordHash(credentials.Password, salt);

            if (user.Password != enteredPasswordHash)
                return null;

            return user;
        }

        public async Task<User> Register(UserDTO userDTO)
        {
            var user = repository.GetUserByEmail(userDTO.Email);

            if (user != null)
                return null;

            var salt = passwordService.GenerateSalt();
            var hashedPassword = passwordService.GeneratePasswordHash(userDTO.Password, salt);

            User registeredUser = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt)
            };

            return await Task.FromResult(repository.CreateUser(registeredUser));
        }

        public User UpdateUser(Guid id, JsonPatchDocument<User> patch)
        {
            var user = repository.GetUserById(id);

            if (user == null)
            {
                return null;
            }

            var detachedUser = MakeUserCopy(user);

            patch.ApplyTo(user);

            if (user.Password != detachedUser.Password)
            {
                user.Password = passwordService.GeneratePasswordHash(user.Password, Convert.FromBase64String(user.Salt));
            }

            repository.UpdateUser(id, user);

            return user;
        }

        private User MakeUserCopy(User user)
        {
            return new User
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Password = user.Password,
            };
        }
    }
}
