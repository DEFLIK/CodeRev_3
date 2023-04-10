using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers.Auth;
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
        private readonly IUserHelper userHelper;

        public InvitationsController(IInvitationValidator invitationValidator, IInvitationCreator invitationCreator, IUserHelper userHelper)
        {
            this.invitationValidator = invitationValidator;
            this.invitationCreator = invitationCreator;
            this.userHelper = userHelper;
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPost("create")]
        public IActionResult Create([Required] [FromHeader(Name = "Authorization")] string authorization, InvitationParams invitationParams)
        {
            //note накопипастил код ниже из ContestController - надо покрасивее сделать
            if (!TokenHelper.TakeUserIdFromAuthHeader(authorization, out var userId))
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            
            var user = userHelper.Get(userId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (user == null)
                return Conflict($"no {nameof(user)} with such id");

            var invitation = invitationCreator.Create(invitationParams, user.Id, out errorString);
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