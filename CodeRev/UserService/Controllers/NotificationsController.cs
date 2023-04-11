using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Enums;
using UserService.Helpers.Auth;
using UserService.Helpers.Notifications;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class NotificationsController : Controller
    {
        private readonly INotificationsHelper notificationsHelper;

        private readonly List<NotificationType> notificationTypesForInterviewer;

        private readonly List<NotificationType> notificationTypesForHr;

        public NotificationsController(INotificationsHelper notificationsHelper)
        {
            this.notificationsHelper = notificationsHelper;

            notificationTypesForInterviewer = new List<NotificationType>
                { NotificationType.UserCreated, NotificationType.InterviewSolutionStarted, NotificationType.InterviewSolutionSubmitted };
            
            notificationTypesForHr = notificationTypesForInterviewer.Concat(new[] { NotificationType.InterviewSolutionChecked }).ToList();
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet]
        public IActionResult GetNotifications([Required][FromHeader(Name = "Authorization")] string authorization)
        {
            Role? role = null;
            if(AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme; //"Bearer"
                var token = headerValue.Parameter; //token
                role = TokenHelper.GetRole(token);
            }
            
            if (role is null)
                return BadRequest();

            var notifications = notificationsHelper.GetReadableNotifications(role == Role.Interviewer ? notificationTypesForInterviewer : notificationTypesForHr);
            return Ok(notifications);
        }
    }
}