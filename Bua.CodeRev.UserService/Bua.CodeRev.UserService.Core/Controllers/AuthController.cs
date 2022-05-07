using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.Models;
using Bua.CodeRev.UserService.DAL;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly DataContext _context;
        
        public AuthController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var user = await AuthenticateUserAsync(request);
            
            if (user == null) 
                return Unauthorized();
            
            return Ok(new
            {
                access_token = GenerateTokenString(user)
            });
        }

        
        [HttpGet("validate-role")]
        public IActionResult ValidateRoleFromToken([Required][FromQuery(Name = "token")] string token)
        {
            if (!IsValidToken(token))
                return Unauthorized();
            var roleClaim = GetClaim(token, ClaimTypes.Role);
            if (roleClaim == null)
                return Unauthorized();
            var role = roleClaim.Value;
            if (!Enum.TryParse(role, true, out RoleEnum roleEnum))
                return Unauthorized();
            return Ok(new
            {
                role = role.ToLower()
            });
        }

        
        [HttpGet("validate-token")]
        public IActionResult ValidateToken([Required][FromQuery(Name = "token")] string token)
        {
            if (!IsValidToken(token))
                return Unauthorized();
            return Ok();
        }
        
        

        private bool IsValidToken(string token)
        {
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = AuthOptions.Issuer,
                    ValidAudience = AuthOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()

                }, out var jwt);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string GenerateTokenString(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            
            var nowTime = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: nowTime,
                claims: claims,
                expires: nowTime.AddSeconds(AuthOptions.Lifetime),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private Claim GetClaim(string token, string claimType) =>
            new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == claimType);

        private async Task<User> AuthenticateUserAsync(LoginRequest request) =>
            await _context.Users
                .FirstOrDefaultAsync(user =>
                    user.Email == request.Email && user.PasswordHash == request.PasswordHash);
    }
}