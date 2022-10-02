using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers;
using UserService.Helpers.Creators;
using UserService.Models.Auth;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class UsersController : ParentController
    {
        private readonly IInterviewCreator interviewCreator;
        private readonly IInvitationValidator invitationValidator;
        
        public UsersController(IDbRepository dbRepository, IInterviewCreator interviewCreator, IInvitationValidator invitationValidator) : base(dbRepository)
        {
            this.interviewCreator = interviewCreator;
            this.invitationValidator = invitationValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([Required][FromQuery(Name = "invite")] string invitationId, 
            [Required][FromBody] UserRegistration userRegistration)
        {
            var invitation = invitationValidator.Validate(invitationId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            
            if (await DbRepository
                    .Get<User>(user => user.Email == userRegistration.Email || user.PhoneNumber == userRegistration.PhoneNumber)
                    .AnyAsync())
                return Conflict("this email or phone number is already registered");

            var userGuid = Guid.NewGuid();
            
            await DbRepository.Add(new User
            {
                Id = userGuid,
                Role = invitation.Role,
                Email = userRegistration.Email,
                PasswordHash = userRegistration.PasswordHash,
                FullName = userRegistration.FullName,
                PhoneNumber = userRegistration.PhoneNumber
            });

            if (invitation.Role != RoleEnum.Candidate)
                await DbRepository.Remove(invitation);

            if (!invitation.InterviewId.Equals(Guid.Empty))
            {
                interviewCreator.CreateSolution(userGuid, invitation.InterviewId);
            }

            await DbRepository.SaveChangesAsync();
            
            return Ok();
        }
    }
}