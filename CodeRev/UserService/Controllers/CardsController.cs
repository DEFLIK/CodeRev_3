using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers.Interviews;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class CardsController : Controller
    {
        private readonly ICardHelper cardHelper;

        public CardsController(ICardHelper cardHelper)
        {
            this.cardHelper = cardHelper;
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet]
        public IActionResult GetInterviewSolutions()
        {
            return Ok(cardHelper.GetCards());
        }
    }
}