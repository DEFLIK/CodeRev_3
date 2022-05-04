using System;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.Models;
using Bua.CodeRev.UserService.DAL;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistration userRegistration)
        {
            if (await IsEmailAlreadyRegisteredAsync(userRegistration.Email)) 
                return Conflict("This email is already registered");
            
            await _context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                Role = RoleEnum.Candidate,
                Email = userRegistration.Email,
                PasswordHash = userRegistration.PasswordHash,
                FullName = userRegistration.FullName,
                PhoneNumber = userRegistration.PhoneNumber
            });

            await _context.SaveChangesAsync();
            
            return Ok();
        }

        private async Task<bool> IsEmailAlreadyRegisteredAsync(string email) =>
            await _context.Users.AnyAsync(user => user.Email == email);
    }
}