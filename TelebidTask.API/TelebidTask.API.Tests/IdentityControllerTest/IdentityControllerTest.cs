using NUnit.Framework;
using Moq;
using TelebidTask.Services.Contracts;
using TelebidTask.API.Controllers;
using TelebidTask.Data.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.JsonPatch;

namespace TelebidTask.API.Tests.IdentityControllerTest
{
    public class IdentityControllerTest
    {
        private Mock<IUserService> mockUserService;
        private IdentityController identityController;

        private Guid validId;

        [SetUp]
        public void Setup()
        {
            mockUserService = new Mock<IUserService>();
            identityController = new IdentityController(mockUserService.Object);

            validId = Guid.Parse("2E232D6E-0DA3-4D96-66D6-08DC9479ACB2");
        }

        [Test]
        public async Task IdentityControllerTest_GetUserById_UserNotFound()
        {
            mockUserService.Setup(x => x.GetUserById(validId)).ReturnsAsync((UserDTO)null);

            var result = await identityController.GetUserById(validId) as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public async Task IdentityControllerTest_GetUserById_UserFound()
        {
            var userDTO = GetSampleUserDTO();

            mockUserService.Setup(x => x.GetUserById(validId)).ReturnsAsync(userDTO);

            var result = await identityController.GetUserById(validId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            var returnedUser = result.Value as UserDTO;

            Assert.IsNotNull(returnedUser);
            Assert.AreEqual(userDTO.Id, returnedUser.Id);
            Assert.AreEqual(userDTO.Name, returnedUser.Name);
            Assert.AreEqual(userDTO.Email, returnedUser.Email);
        }

        [Test]
        public async Task IdentityControllerTest_UpdateUser_NullPatch()
        {
            JsonPatchDocument<User> patch = null;

            var result = await identityController.UpdateUser(validId, patch) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
        }

        [Test]
        public async Task IdentityControllerTest_UpdateUser_InvalidUser()
        {
            var patch = new JsonPatchDocument<User>();

            mockUserService.Setup(x => x.UpdateUser(validId, patch)).ReturnsAsync((UserDTO)null);

            var result = await identityController.UpdateUser(validId, patch) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
        }

        [Test]
        public async Task IdentityControllerTest_UpdateUser_ValidUser()
        {
            var userDTO = GetSampleUserDTO();
            var patch = new JsonPatchDocument<User>();

            mockUserService.Setup(x => x.UpdateUser(validId, patch)).ReturnsAsync(userDTO);

            var result = await identityController.UpdateUser(validId, patch) as NoContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task IdentityControllerTest_Register_InvalidRegistrationModel()
        {
            var result = await identityController.Register(null) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
        }

        [Test]
        public async Task IdentityControllerTest_Register_UnsuccessfulRegistration()
        {
            var registrationModel = GetSampleRegistrationModel();

            mockUserService.Setup(x => x.Register(registrationModel)).ReturnsAsync((UserDTO)null);

            var result = await identityController.Register(registrationModel) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
        }

        [Test]
        public async Task IdentityControllerTest_Register_SuccessfulRegistration()
        {
            var userDTO = GetSampleUserDTO();
            var registrationModel = GetSampleRegistrationModel();

            mockUserService.Setup(x => x.Register(registrationModel)).ReturnsAsync(userDTO);

            var result = await identityController.Register(registrationModel) as OkResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task IdentityControllerTest_Login_InvalidLoginCredentials()
        {
            var result = await identityController.Login(null) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
        }

        [Test]
        public async Task IdentityControllerTest_Login_UnsuccessfulLogin()
        {
            var loginCredentials = GetSampleLoginCredentials();

            mockUserService.Setup(x => x.Login(loginCredentials)).ReturnsAsync((UserDTO)null);

            var result = await identityController.Login(loginCredentials) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
        }

        [Test]
        public async Task IdentityControllerTest_Login_SuccessfulLogin()
        {
            var userDTO = GetSampleUserDTO();
            var loginCredentials = GetSampleLoginCredentials();

            mockUserService.Setup(x => x.Login(loginCredentials)).ReturnsAsync(userDTO);

            var result = await identityController.Login(loginCredentials) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            var returnedObj = result.Value;

            Assert.IsNotNull(returnedObj);
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
    }
}