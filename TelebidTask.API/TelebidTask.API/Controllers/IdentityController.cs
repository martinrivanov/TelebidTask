using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TelebidTask.Data.Models;
using TelebidTask.Services.Contracts;

namespace TelebidTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService userService;

        public IdentityController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("/{id}")]
        public IActionResult GetUserById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var user = userService.GetUserById(id);

            if(user == null)
            {
                return NotFound();
            }

            return new OkObjectResult(user);
        }

        [HttpPatch]
        [Route("/")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody]JsonPatchDocument<User> patch)
        {
            if (patch == null || id == null || !ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Mesage = "No Credentials" });
            }

            var user = userService.UpdateUser(id, patch);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Mesage = "No User Was Found" });
            }

            return new OkObjectResult(user);
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO registration)
        {
            if (registration == null || !ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Mesage = "No Credentials" });
            }

            var newUser = await userService.Register(registration);

            if (newUser == null)
            {
                return new BadRequestObjectResult(new { Mesage = "Registration Unsuccessful" });
            }

            return new OkResult();
        }

        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {
            if (credentials == null || !ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Mesage = "No Credentials" });
            }

            var user = userService.Login(credentials);

            if (user == null)
            {
                return new BadRequestObjectResult(new { Message = "Incorrect email or password" });
            }

            return new OkObjectResult(new { UserId = user.Id });
        }
    }
}
