using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.Models;
using Bua.CodeRev.UserService.DAL;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([Required][FromQuery(Name = "invite")] string invitationId, 
            [Required][FromBody] UserRegistration userRegistration)
        {
            var invitationGuid = new Guid();
            try
            {
                invitationGuid = Guid.Parse(invitationId);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("invitation id to be parsed is null.");
            }
            catch (FormatException)
            {
                return BadRequest("invitation id should be in UUID format");
            }
            
            var invitation = await _context.Invitations.FindAsync(invitationGuid);
            if (invitation == null)
                return Conflict("this invitation doesn't exist or is expired");
            if (invitation.ExpiredAt < DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                _context.Invitations.Remove(invitation);
                await _context.SaveChangesAsync();
                return Conflict("this invitation doesn't exist or is expired");
            }

            if (await _context.Users
                .AnyAsync(user => user.Email == userRegistration.Email || user.PhoneNumber == userRegistration.PhoneNumber)) 
                return Conflict("this email or phone number is already registered");

            var userGuid = Guid.NewGuid();
            
            await _context.Users.AddAsync(new User
            {
                Id = userGuid,
                Role = invitation.Role,
                Email = userRegistration.Email,
                PasswordHash = userRegistration.PasswordHash,
                FullName = userRegistration.FullName,
                PhoneNumber = userRegistration.PhoneNumber
            });

            if (invitation.Role != RoleEnum.Candidate)
                _context.Invitations.Remove(invitation);

            await _context.SaveChangesAsync();

            if (!invitation.InterviewId.Equals(Guid.Empty))
                CreateInterviewSolutionAsync(userGuid, invitation.InterviewId);
            
            return Ok();
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPost("create-invitation")]
        public async Task<IActionResult> CreateInvitationGuidAsync(InvitationParams invitationParams)
        {
            if (!Enum.TryParse(invitationParams.Role, true, out RoleEnum roleEnum))
                return BadRequest("role is invalid");
            
            var mustBeId = invitationParams.InterviewId != null || roleEnum == RoleEnum.Candidate;
            var interviewGuid = new Guid();
            if (mustBeId)
            {
                try
                {
                    interviewGuid = Guid.Parse(invitationParams.InterviewId);
                }
                catch (ArgumentNullException)
                {
                    return BadRequest("interview id to be parsed is null");
                }
                catch (FormatException)
                {
                    return BadRequest("interview id should be in UUID format");
                }
            }

            if (mustBeId && await _context.Interviews.FindAsync(interviewGuid) == null)
                return BadRequest("no interview with such id");

            var invitationGuid = Guid.NewGuid();
            var invitation = new Invitation
            {
                Id = invitationGuid,
                Role = roleEnum,
                InterviewId = interviewGuid,
                ExpiredAt = DateTimeOffset.Now.ToUnixTimeMilliseconds() + (long) (6 * Math.Pow(10, 8))
            };
            
            await _context.Invitations.AddAsync(invitation);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                invitation = $"/api/users/register?invite={invitationGuid}"
            });
        }

        private async void CreateInterviewSolutionAsync(Guid userGuid, Guid interviewGuid)
        {
            var interviewSolutionGuid = Guid.NewGuid();
            await _context.InterviewSolutions.AddAsync(new InterviewSolution
            {
                Id = interviewSolutionGuid,
                UserId = userGuid,
                InterviewId = interviewGuid,
                StartTimeMs = -1,
                EndTimeMs = -1,
                TimeToCheckMs = -1,
                ReviewerComment = "",
                InterviewResult = InterviewResultEnum.NotChecked
            });

            foreach (var taskId in _context.InterviewTasks
                .Where(it => it.InterviewId == interviewGuid)
                .Select(it => it.TaskId))
            {
                CreateTaskSolutionAsync(interviewSolutionGuid, taskId);
            }

            await _context.SaveChangesAsync();
        }

        private async void CreateTaskSolutionAsync(Guid interviewSolutionGuid, Guid taskGuid)
        {
            await _context.TaskSolutions.AddAsync(new TaskSolution
            {
                Id = Guid.NewGuid(),
                InterviewSolutionId = interviewSolutionGuid,
                TaskId = taskGuid,
                IsDone = false,
                TaskGrade = TaskGradeEnum.Zero,
            });
        }
    }
}