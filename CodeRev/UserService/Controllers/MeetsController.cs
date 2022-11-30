using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers;

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
        public IActionResult GetMeets()
        {
            return Ok(meetsHelper.GetMeets());
        }
    }
}