using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await userService.GetUserById(id);

            if(user == null)
            {
                return NotFound();
            }

            return new OkObjectResult(user);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPatch]
        [Route("/")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody]JsonPatchDocument<User> patch)
        {
            if (patch == null || !ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { Mesage = "Invalid Data" });
            }

            var user = await userService.UpdateUser(id, patch);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Mesage = "No User Was Found" });
            }

            return new NoContentResult();
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationUserModel registration)
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

            var user = await userService.Login(credentials);

            if (user == null)
            {
                return new BadRequestObjectResult(new { Message = "Incorrect email or password" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return new OkObjectResult(new { UserId = user.Id });
        }

        [HttpPost]
        [Route("/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new OkResult();
        }
    }
}
