using System;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewsController : Controller
    {
        private readonly DataContext _context;
        
        public InterviewsController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet("start-interview-sln")]
        public async Task<IActionResult> StartInterviewSolutionAsync([FromQuery(Name = "id")] string interviewSolutionId)
        {
            var interviewSolutionGuid = new Guid();
            try
            {
                interviewSolutionGuid = Guid.Parse(interviewSolutionId);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("interview solution id to be parsed is null.");
            }
            catch (FormatException)
            {
                return BadRequest("interview solution id should be in UUID format");
            }
            
            var interviewSolution = await _context.InterviewSolutions.FindAsync(interviewSolutionGuid);
            if (interviewSolution == null)
                return BadRequest("No interview solution with this uuid");
                
            var interviewTask = _context.Interviews.FirstOrDefaultAsync(iv => iv.Id == interviewSolution.InterviewId);
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            interviewSolution.StartTimeMs = nowTime;
                
            var interview = await interviewTask;
            if (interview == null)
                return BadRequest("No such interview");
                
            interviewSolution.EndTimeMs = nowTime + interview.InterviewDurationMs;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}