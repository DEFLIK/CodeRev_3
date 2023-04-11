using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers;
using UserService.Helpers.Auth;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class MeetsController : Controller
    {
        private readonly IMeetsHelper meetsHelper;

        public MeetsController(IMeetsHelper meetsHelper)
        {
            this.meetsHelper = meetsHelper;
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet]
        public IActionResult GetMeets([Required] [FromHeader(Name = "Authorization")] string authorization)
        {
            //note накопипастил код ниже из ContestController - надо покрасивее сделать
            if (!TokenHelper.TakeUserIdFromAuthHeader(authorization, out var userId))
                return BadRequest($"Unexpected {nameof(authorization)} header value");

            return Ok(meetsHelper.GetMeets(userId));
        }
    }
}