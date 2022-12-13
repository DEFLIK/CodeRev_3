using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers.Auth.Invitations;
using UserService.Models.Users;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class InvitationsController : Controller
    {
        private readonly IInvitationValidator invitationValidator;
        private readonly IInvitationCreator invitationCreator;

        public InvitationsController(IInvitationValidator invitationValidator, IInvitationCreator invitationCreator)
        {
            this.invitationValidator = invitationValidator;
            this.invitationCreator = invitationCreator;
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPost("create")]
        public IActionResult Create(InvitationParams invitationParams)
        {
            var invitation = invitationCreator.Create(invitationParams, out var errorString);
            if (errorString != null && invitation == null)
                return BadRequest(errorString);
            
            return Ok(new
            {
                invitation = invitation.Id
            });
        }

        [HttpGet("validate")]
        public IActionResult ValidateInvitationAsync([Required] [FromQuery(Name = "invite")] string invitationId)
        {
            invitationValidator.Validate(invitationId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);

            return Ok();
        }
    }
}