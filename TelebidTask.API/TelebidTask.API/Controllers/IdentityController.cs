using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TelebidTask.Data;
using TelebidTask.Data.Models;
using TelebidTask.Services.Contracts;

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

        [HttpGet]
        [Route("/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);

            if(user == null)
            {
                return BadRequest();
            }

            return new OkObjectResult(user);
        }

        [HttpPatch]
        [Route("/")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody]JsonPatchDocument<User> patch)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return BadRequest();
            }

            var detachedUser = new User
            {
                Id = id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
            };

            patch.ApplyTo(user, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await context.SaveChangesAsync();

            if (detachedUser.Password != user.Password)
            {
                user.Password = passwordService.GeneratePasswordHash(user.Password, Convert.FromBase64String(user.Salt));
                await context.SaveChangesAsync();
            }

            return new OkObjectResult(new{ user, Password = user.Password});
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
