using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.LogicHelpers;
using UserService.Models.Auth;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class AuthController : ParentController
    {
        private readonly TokenHelper _tokenHelper;
        
        public AuthController(IDbRepository dbRepository) : base(dbRepository)
        {
            _tokenHelper = new TokenHelper();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var user = await AuthenticateUserAsync(request);
            
            if (user == null) 
                return Unauthorized();
            
            return Ok(new
            {
                accessToken = _tokenHelper.GenerateTokenString(user)
            });
        }
        
        [HttpGet("validate-role")]
        public IActionResult ValidateRoleFromToken([Required][FromQuery(Name = "token")] string token)
        {
            if (!_tokenHelper.IsValidToken(token))
                return Unauthorized();
            var roleClaim = _tokenHelper.GetClaim(token, "role");
            if (roleClaim == null)
                return Unauthorized();
            var role = roleClaim.Value;
            if (!Enum.TryParse(role, true, out RoleEnum _))
                return Unauthorized();
            return Ok(new
            {
                role = role.ToLower()
            });
        }
        
        [HttpGet("validate-token")]
        public IActionResult ValidateToken([Required][FromQuery(Name = "token")] string token)
        {
            if (!_tokenHelper.IsValidToken(token))
                return Unauthorized();
            return Ok();
        }

        private async Task<User> AuthenticateUserAsync(LoginRequest request) =>
            await _dbRepository
                .Get<User>(user => user.Email == request.Email && user.PasswordHash == request.PasswordHash)
                .FirstOrDefaultAsync();
    }
}