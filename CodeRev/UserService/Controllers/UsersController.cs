using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers.Auth;
using UserService.Models.Auth;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserCreator userCreator;
        
        public UsersController(IUserCreator userCreator)
        {
            this.userCreator = userCreator;
        }

        [HttpPost("register")]
        public IActionResult Register([Required][FromQuery(Name = "invite")] string invitationId, 
            [Required][FromBody] UserRegistration userRegistration)
        {
            userCreator.Create(userRegistration, invitationId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            
            return Ok();
        }
        
        [HttpPost("registerViaVk")]
        public IActionResult RegisterViaVk(
            [Required][FromQuery(Name = "invite")] string invitationId, 
            [Required][FromBody] UserVkRegistration userRegistration)
        {
            TokenHelper.IsValidVkSession(userRegistration.VkSession);
            
            userCreator.Create(userRegistration, invitationId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            
            return Ok();
        }
    }
}