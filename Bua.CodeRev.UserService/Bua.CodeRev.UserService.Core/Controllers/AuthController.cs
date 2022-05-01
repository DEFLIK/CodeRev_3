﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Bua.CodeRev.UserService.Core.Models;
using Bua.CodeRev.UserService.DAL;
using Bua.CodeRev.UserService.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtConstants = System.IdentityModel.Tokens.Jwt.JwtConstants;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly DataContext _context;
        
        public AuthController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = AuthenticateUser(request);
            if (user == null) 
                return Unauthorized();
            
            var jwtToken = GenerateTokenString(user);
            return Ok(new
            {
                access_token = jwtToken
            });
        }

        private string GenerateTokenString(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };


            var nowTime = DateTime.UtcNow;
            var jwtToken = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: nowTime,
                claims: claims,
                expires: nowTime.AddSeconds(AuthOptions.Lifetime),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private User AuthenticateUser(LoginRequest request)
        {
            var userTask = _context.Users
                .FirstOrDefaultAsync(user =>
                    user.Email == request.Email && user.PasswordHash == request.PasswordHash);
            userTask.Wait();
            return userTask.Result;
        }
    }
}