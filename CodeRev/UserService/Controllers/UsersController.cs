using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Enums;
using UserService.Helpers.Auth;
using UserService.Helpers.Notifications;
using UserService.Models.Auth;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserCreator userCreator;
        private readonly IUserHelper userHelper;
        private readonly INotificationsHelper notificationsHelper;

        public UsersController(IUserCreator userCreator, IUserHelper userHelper, INotificationsHelper notificationsHelper)
        {
            this.userCreator = userCreator;
            this.userHelper = userHelper;
            this.notificationsHelper = notificationsHelper;
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

        [HttpGet("all")]
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        public IActionResult GetAllUsers() => 
            Ok(userHelper.GetAll());

        [HttpPut("")]
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        public async Task EditUser(
            [Required][FromQuery(Name = "userEmail")] string userEmail,
            [FromQuery(Name = "userName")] string userName,
            [FromQuery(Name = "userRole")] Role userRole)
        {
            await userHelper.EditUser(userEmail, userName, userRole);
        }
        
        // TODO удалять остатки работы пользователя - завязанные решени и тд?
        [HttpDelete("")]
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        public async Task DeleteUser(
            [Required][FromQuery(Name = "userEmail")] string userEmail)
        {
            var userId = await userHelper.DeleteUser(userEmail);
            await notificationsHelper.DeleteUserNotifications(userId);
        }
    }
}