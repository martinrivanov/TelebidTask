using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelebidTask.Data;
using TelebidTask.Data.Models;
using TelebidTasks.Services.Contracts;

namespace TelebidTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IPasswordService passwordService;

        public IdentityController(DataContext context, IPasswordService passwordService)
        {
            this.context = context;
            this.passwordService = passwordService;
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registration)
        {
            if (registration == null || !ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Mesage = "No Credentials" });
            }

            var user = context.Users.FirstOrDefault(u => u.Email.Equals(registration.Email));

            if (user != null)
            {
                return new BadRequestObjectResult(new { Mesage = "Already Existent User" });
            }

            var salt = passwordService.GenerateSalt();
            var hashedPassword = passwordService.GeneratePasswordHash(registration.Password, salt);

            User registeredUser = new User
            {
                Name = registration.Name,
                Email = registration.Email,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt)
            };

            await context.Users.AddAsync(registeredUser);
            await context.SaveChangesAsync();

            return new OkObjectResult(new { Message = "Successful Registration"});
        }

        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {
            if (credentials == null || !ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Mesage = "No Credentials" });
            }

            var user = context.Users.FirstOrDefault(u => u.Email.Equals(credentials.Email));

            if (user == null)
            {
                return new BadRequestObjectResult(new { Mesage = "Non-Existent User" });
            }

            var salt = Convert.FromBase64String(user.Salt);
            var enteredPasswordHash = passwordService.GeneratePasswordHash(credentials.Password, salt);

            if (user.Password != enteredPasswordHash)
            {
                return new BadRequestObjectResult(new { Mesage = "Incorrect Password" });
            }

            return new OkObjectResult(new { Message = "Successful Login", User = user});
        }
    }
}
