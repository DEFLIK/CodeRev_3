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
        private readonly ITokenHelper tokenHelper;
        private readonly IUserHelper userHelper;
        
        public AuthController(ITokenHelper tokenHelper, IUserHelper userHelper)
        {
            this.tokenHelper = tokenHelper;
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
                accessToken = tokenHelper.GenerateTokenString(user)
            });
        }
        
        [HttpGet("validate-role")]
        public IActionResult ValidateRole([Required][FromQuery(Name = "token")] string token)
        {
            var role = tokenHelper.GetRole(token);
            if (role == null)
                return Unauthorized();
            
            return Ok(new
            {
                role = role.ToString()?.ToLower()
            });
        }
        
        [HttpGet("validate")]
        public IActionResult ValidateToken([Required][FromQuery(Name = "token")] string token)
            => tokenHelper.IsValidToken(token) ? Ok() : Unauthorized();
    }
}