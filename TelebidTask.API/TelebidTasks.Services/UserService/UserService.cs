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

        public async Task<UserDTO> GetUserById(Guid Id)
        {
            var user = await repository.GetUserById(Id);

            if (user == null)
            {
                return null;
            }

            return MapUserToUserDTO(user);
        }

        public async Task<UserDTO> Login(LoginCredentials credentials)
        {
            var user = await repository.GetUserByEmail(credentials.Email);

            if (user == null)
                return null;

            var salt = Convert.FromBase64String(user.Salt);
            var enteredPasswordHash = passwordService.GeneratePasswordHash(credentials.Password, salt);

            if (user.Password != enteredPasswordHash)
                return null;

            return MapUserToUserDTO(user);
        }

        public async Task<UserDTO> Register(RegistrationUserModel userDTO)
        {
            var user = await repository.GetUserByEmail(userDTO.Email);

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

            var newUser = await repository.CreateUser(registeredUser);

            if (newUser == null)
            {
                return null;
            }

            return MapUserToUserDTO(newUser);
        }

        public async Task<UserDTO> UpdateUser(Guid id, JsonPatchDocument<User> patch)
        {
            var user = await repository.GetUserById(id);

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

            await repository.UpdateUser(id, user);

            return MapUserToUserDTO(user);
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

        private UserDTO MapUserToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}
