using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.Models.Auth;
using Bua.CodeRev.UserService.Core.Models.Users;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models;
using Bua.CodeRev.UserService.DAL.Models.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class UsersController : ParentController
    {
        private const long invitationDuration = 604800000; // == 1 week //todo make config setting
        
        public UsersController(IDbRepository dbRepository) : base(dbRepository)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([Required][FromQuery(Name = "invite")] string invitationId, 
            [Required][FromBody] UserRegistration userRegistration)
        {
            var (invitationGuid, errorString) = TryParseGuid(invitationId, nameof(invitationId));
            if (errorString != null)
                return BadRequest(errorString);
            
            var invitation = await _dbRepository
                .Get<Invitation>(i => i.Id == invitationGuid)
                .FirstOrDefaultAsync();
            
            if (invitation == null)
                return Conflict("this invitation doesn't exist or is expired");
            
            if (invitation.ExpiredAt < DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                await _dbRepository.Remove(invitation);
                await _dbRepository.SaveChangesAsync();
                return Conflict("this invitation doesn't exist or is expired");
            }
            
            if (await _dbRepository
                .Get<User>(user => user.Email == userRegistration.Email || user.PhoneNumber == userRegistration.PhoneNumber)
                .AnyAsync())
                return Conflict("this email or phone number is already registered");

            var userGuid = Guid.NewGuid();
            
            await _dbRepository.Add(new User
            {
                Id = userGuid,
                Role = invitation.Role,
                Email = userRegistration.Email,
                PasswordHash = userRegistration.PasswordHash,
                FullName = userRegistration.FullName,
                PhoneNumber = userRegistration.PhoneNumber
            });

            if (invitation.Role != RoleEnum.Candidate)
                await _dbRepository.Remove(invitation);

            if (!invitation.InterviewId.Equals(Guid.Empty))
            {
                await CreateInterviewSolutionAsync(userGuid, invitation.InterviewId);
            }

            await _dbRepository.SaveChangesAsync();
            
            return Ok();
        }
        
        //[Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPost("create-invitation")]
        public async Task<IActionResult> CreateInvitationAsync(InvitationParams invitationParams)
        {
            if (!Enum.TryParse(invitationParams.Role, true, out RoleEnum roleEnum))
                return BadRequest("role is invalid");
            
            var mustBeInterviewId = invitationParams.InterviewId != null || roleEnum == RoleEnum.Candidate;
            var interviewGuid = Guid.Empty;
            if (mustBeInterviewId)
            {
                string errorString;
                (interviewGuid, errorString) = TryParseGuid(invitationParams.InterviewId, nameof(invitationParams.InterviewId));
                if (errorString != null)
                    return BadRequest(errorString);
            }

            if (mustBeInterviewId && await _dbRepository
                .Get<Interview>(i => i.Id == interviewGuid)
                .FirstOrDefaultAsync() == null)
                return Conflict("no interview with such id");

            var invitation = await _dbRepository
                .Get<Invitation>(i => i.Role == roleEnum && i.InterviewId == interviewGuid)
                .FirstOrDefaultAsync();

            var invitationGuid = Guid.NewGuid();
            if (invitation == null)
            {
                invitation = new Invitation
                {
                    Id = invitationGuid,
                    Role = roleEnum,
                    InterviewId = interviewGuid,
                    ExpiredAt = DateTimeOffset.Now.ToUnixTimeMilliseconds() + invitationDuration
                };
                await _dbRepository.Add(invitation);
            }
            else
            {
                invitationGuid = invitation.Id;
                invitation.ExpiredAt = DateTimeOffset.Now.ToUnixTimeMilliseconds() + invitationDuration;
            }
            
            await _dbRepository.SaveChangesAsync();

            return Ok(new
            {
                invitation = invitationGuid
            });
        }

        [HttpGet("validate-invitation")]
        public async Task<IActionResult> ValidateInvitationAsync([Required] [FromQuery(Name = "invite")] string invitationId)
        {
            var (invitationGuid, errorString) = TryParseGuid(invitationId, nameof(invitationId));
            if (errorString != null)
                return BadRequest(errorString);
            var invitation = await _dbRepository
                .Get<Invitation>(i => i.Id == invitationGuid)
                .FirstOrDefaultAsync();
            
            if (invitation == null)
                return Conflict("this invitation doesn't exist or is expired");
            
            if (invitation.ExpiredAt < DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                await _dbRepository.Remove(invitation);
                await _dbRepository.SaveChangesAsync();
                return Conflict("this invitation doesn't exist or is expired");
            }

            return Ok();
        }

        private async System.Threading.Tasks.Task CreateInterviewSolutionAsync(Guid userGuid, Guid interviewGuid)
        {
            var interviewSolutionGuid = Guid.NewGuid();
            await _dbRepository.Add(new InterviewSolution
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

            foreach (var taskId in _dbRepository
                .Get<InterviewTask>(it => it.InterviewId == interviewGuid)
                .Select(it => it.TaskId))
            {
                await CreateTaskSolutionAsync(interviewSolutionGuid, taskId);
            }
        }

        private async System.Threading.Tasks.Task CreateTaskSolutionAsync(Guid interviewSolutionGuid, Guid taskGuid)
        {
            await _dbRepository.Add(new TaskSolution
            {
                Id = Guid.NewGuid(),
                InterviewSolutionId = interviewSolutionGuid,
                TaskId = taskGuid,
                IsDone = false,
                Grade = GradeEnum.Zero,
            });
        }
    }
}