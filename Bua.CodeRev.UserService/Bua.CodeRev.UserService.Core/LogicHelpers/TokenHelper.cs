using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Bua.CodeRev.UserService.DAL.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Bua.CodeRev.UserService.Core.LogicHelpers
{
    public class TokenHelper
    {
        public bool IsValidToken(string token)
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

                }, out _);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public string GenerateTokenString(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role.ToString())
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

        public Claim GetClaim(string token, string claimType) =>
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token)
                .Claims
                .FirstOrDefault(c => c.Type == claimType);

        public string TakeUserIdFromToken(string token) => 
            IsValidToken(token) ? GetClaim(token, JwtRegisteredClaimNames.Sub)?.Value : null;
    }
}