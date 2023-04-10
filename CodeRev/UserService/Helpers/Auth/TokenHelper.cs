using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;

namespace UserService.Helpers.Auth
{
    public static class TokenHelper
    {
        public static bool IsValidToken(string token)
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

        public static string GenerateTokenString(User user)
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

        public static Claim GetClaim(string token, string claimType) =>
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token)
                .Claims
                .FirstOrDefault(c => c.Type == claimType);

        public static Role? GetRole(string token)
        {
            if (!IsValidToken(token))
                return null;
            
            var roleClaim = GetClaim(token, "role");
            if (roleClaim == null)
                return null;

            if (!Enum.TryParse(roleClaim.Value, true, out Role role))
                return null;

            return role;
        }

        public static string TakeUserIdFromToken(string token)
            => IsValidToken(token) ? GetClaim(token, JwtRegisteredClaimNames.Sub)?.Value : null;

        public static bool TakeUserIdFromAuthHeader(string header, out string userId)
        {
            userId = null;
            if (!header.StartsWith("Bearer"))
                return false;
            var splitValue = header.Split();
            if (splitValue.Length != 2)
                return false;
            
            userId = TakeUserIdFromToken(splitValue[1]);
            return userId != null;
        }
    }
}