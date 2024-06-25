using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TelebidTask.Data.Contracts;
using TelebidTask.Data.Models;
using TelebidTask.Services.Contracts;

namespace TelebidTask.Services.Tests.UserServiceTest
{
    public class UserServiceTest
    {
        private Mock<IDatabaseRepository> mockDatabaseRepository;
        private Mock<IPasswordService> mockPasswordService;
        private UserService.UserService userService;

        private Guid validId;

        [SetUp]
        public void SetUp()
        {
            this.mockDatabaseRepository = new Mock<IDatabaseRepository>();
            this.mockPasswordService = new Mock<IPasswordService>();
            this.userService = new UserService.UserService(this.mockDatabaseRepository.Object, this.mockPasswordService.Object);

            validId = Guid.Parse("2E232D6E-0DA3-4D96-66D6-08DC9479ACB2");
        }

        [Test]
        public async Task UserServiceTest_GetUserById_ValidId()
        {
            mockDatabaseRepository.Setup(x => x.GetUserById(validId)).ReturnsAsync(GetSampleUser());

            var expected = GetSampleUserDTO();
            var result = await userService.GetUserById(validId);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Email, result.Email);
        }

        [Test]
        public async Task UserServiceTest_GetUserById_InvalidId()
        {
            mockDatabaseRepository.Setup(x => x.GetUserById(validId)).ReturnsAsync((User)null);

            var result = await userService.GetUserById(validId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UserServiceTest_Login_InvalidEmail()
        {
            var loginCredentials = GetSampleLoginCredentials();
            mockDatabaseRepository.Setup(x => x.GetUserByEmail(loginCredentials.Email)).ReturnsAsync((User)null);

            var result = await userService.Login(loginCredentials);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UserServiceTest_Login_ValidEmailInvalidPassword()
        {
            var user = GetSampleUser();
            var loginCredentials = GetSampleLoginCredentials();
            loginCredentials.Password = "testtest124";

            mockDatabaseRepository.Setup(x => x.GetUserByEmail(loginCredentials.Email)).ReturnsAsync(user);
            mockPasswordService.Setup(x => x.GeneratePasswordHash(loginCredentials.Password, It.IsAny<byte[]>())).Returns(loginCredentials.Password);

            var result = await userService.Login(loginCredentials);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UserServiceTest_Login_ValidEmailValidPassword()
        {
            var user = GetSampleUser();
            var userDTO = GetSampleUserDTO();
            var loginCredentials = GetSampleLoginCredentials();

            mockDatabaseRepository.Setup(x => x.GetUserByEmail(loginCredentials.Email)).ReturnsAsync(user);
            mockPasswordService.Setup(x => x.GeneratePasswordHash(loginCredentials.Password, It.IsAny<byte[]>())).Returns(loginCredentials.Password);

            var result = await userService.Login(loginCredentials);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDTO.Id, result.Id);
            Assert.AreEqual(userDTO.Name, result.Name);
            Assert.AreEqual(userDTO.Email, result.Email);
        }

        
        [Test]
        public async Task UserServiceTest_Register_AlreadyExistingUser()
        {
            var registrationModel = GetSampleRegistrationModel();
            var user = GetSampleUser();

            mockDatabaseRepository.Setup(x => x.GetUserByEmail(registrationModel.Email)).ReturnsAsync(user);

            var result = await userService.Register(registrationModel);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UserServiceTest_Register_UserCouldNotBeAddedToDatabase()
        {
            var registrationModel = GetSampleRegistrationModel();
            var userDTO = GetSampleUserDTO();
            var user = GetSampleUser();
            var salt = new byte[16];

            mockDatabaseRepository.Setup(x => x.GetUserByEmail(registrationModel.Email)).ReturnsAsync((User)null);
            mockPasswordService.Setup(x => x.GenerateSalt()).Returns(salt);
            mockPasswordService.Setup(x => x.GeneratePasswordHash(user.Password, salt)).Returns(user.Password);
            mockDatabaseRepository.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync((User)null);

            var result = await userService.Register(registrationModel);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UserServiceTest_Register_NewlyCreatedUser()
        {
            var registrationModel = GetSampleRegistrationModel();
            var userDTO = GetSampleUserDTO();
            var user = GetSampleUser();
            var salt = new byte[16];

            mockDatabaseRepository.Setup(x => x.GetUserByEmail(registrationModel.Email)).ReturnsAsync((User)null);
            mockPasswordService.Setup(x => x.GenerateSalt()).Returns(salt);
            mockPasswordService.Setup(x => x.GeneratePasswordHash(user.Password, It.IsAny<byte[]>())).Returns(user.Password);
            mockDatabaseRepository.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync(user);

            var result = await userService.Register(registrationModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDTO.Id, result.Id);
            Assert.AreEqual(userDTO.Name, result.Name);
            Assert.AreEqual(userDTO.Email, result.Email);
        }

        [Test]
        public async Task UserServiceTest_UpdateUser_NonExistentUser()
        {
            var patch = new JsonPatchDocument<User>();

            mockDatabaseRepository.Setup(x => x.GetUserById(validId)).ReturnsAsync((User)null);

            var result = await userService.UpdateUser(validId, patch);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UserServiceTest_UpdateUser_ExistingUser()
        {
            var user = GetSampleUser();
            var newEmail = "newemail@mail.bg";

            var patch = new JsonPatchDocument<User>();
            patch.Replace(u => u.Email, newEmail);

            mockDatabaseRepository.Setup(x => x.GetUserById(validId)).ReturnsAsync(user);
            mockPasswordService.Setup(x => x.GeneratePasswordHash(user.Password, It.IsAny<byte[]>())).Returns(user.Password);
            mockDatabaseRepository.Setup(x => x.UpdateUser(user.Id, user)).Returns(Task.CompletedTask);


            var result = await userService.UpdateUser(validId, patch);

            Assert.IsNotNull(result);
            Assert.AreEqual(newEmail, result.Email);
        }


        private User GetSampleUser()
        {
            return new User
            {
                Id = validId,
                Name = "TestName",
                Email = "test@test.com",
                Password = "testtest123",
                Salt = Convert.ToBase64String(new byte[16])
            };
        }

        private LoginCredentials GetSampleLoginCredentials()
        {
            return new LoginCredentials
            {
                Email = "test@test.com",
                Password = "testtest123",
            };
        }

        private RegistrationUserModel GetSampleRegistrationModel()
        {
            return new RegistrationUserModel
            {
                Name = "TestName",
                Email = "test@test.com",
                Password = "testtest123",
            };
        }

        private UserDTO GetSampleUserDTO()
        {
            return new UserDTO
            {
                Id = validId,
                Name = "TestName",
                Email = "test@test.com",
            };
        }
    }
}
