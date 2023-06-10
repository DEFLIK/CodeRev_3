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
    public class AuthController : Controller
    {
        private readonly IUserHelper userHelper;
        
        public AuthController(IUserHelper userHelper)
        {
            this.userHelper = userHelper;
        }
        
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = userHelper.Get(request);
            
            if (user == null) 
                return Unauthorized();
            
            return Ok(new
            {
                accessToken = TokenHelper.GenerateTokenString(user)
            });
        }
        
        [HttpPost("loginViaVk")]
        public IActionResult LoginViaVk([Required][FromQuery] string vkId, [FromBody] VkSession session)
        {
            if (!TokenHelper.IsValidVkSession(session))
                return Unauthorized();
            
            var user = userHelper.Get(new LoginRequest()
            {
                Email = vkId,
                PasswordHash = TokenHelper.VkMockPassHash
            });
            
            if (user == null) 
                return Unauthorized();
            
            return Ok(new
            {
                accessToken = TokenHelper.GenerateTokenString(user)
            });
        }
        
        [HttpGet("validate-role")]
        public IActionResult ValidateRole([Required][FromQuery(Name = "token")] string token)
        {
            var role = TokenHelper.GetRole(token);
            if (role == null)
                return Unauthorized();
            
            return Ok(new
            {
                role = role.ToString()?.ToLower()
            });
        }
        
        [HttpGet("validate")]
        public IActionResult ValidateToken([Required][FromQuery(Name = "token")] string token)
            => TokenHelper.IsValidToken(token) ? Ok() : Unauthorized();
        
        [HttpGet("validateVk")]
        public IActionResult ValidateVkSession([Required][FromBody] VkSession vkSession)
            => TokenHelper.IsValidVkSession(vkSession) ? Ok() : Unauthorized();
    }
}