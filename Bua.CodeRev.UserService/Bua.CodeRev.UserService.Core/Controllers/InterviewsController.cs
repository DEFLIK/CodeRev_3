using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.DAL;
using Microsoft.AspNetCore.Authorization;
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
        
        [Authorize]
        [HttpGet("start-interview-sln")]
        public async Task<IActionResult> StartInterviewSolutionAsync([Required][FromQuery(Name = "id")] string interviewSolutionId)
        {
            var interviewSolutionGuid = new Guid();
            try
            {
                interviewSolutionGuid = Guid.Parse(interviewSolutionId);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("interview solution id to be parsed is null");
            }
            catch (FormatException)
            {
                return BadRequest("interview solution id should be in UUID format");
            }
            
            var interviewSolution = await _context.InterviewSolutions.FindAsync(interviewSolutionGuid);
            if (interviewSolution == null)
                return BadRequest("no interview solution with such id");
                
            var interviewTask = _context.Interviews.FirstOrDefaultAsync(iv => iv.Id == interviewSolution.InterviewId);
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            interviewSolution.StartTimeMs = nowTime;
                
            var interview = await interviewTask;
            if (interview == null)
                return BadRequest("no interview with such id");
                
            interviewSolution.EndTimeMs = nowTime + interview.InterviewDurationMs;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [Authorize]
        [HttpGet("end-interview-sln")]
        public async Task<IActionResult> EndInterviewSolutionAsync([Required][FromQuery(Name = "id")] string interviewSolutionId)
        {
            var interviewSolutionGuid = new Guid();
            try
            {
                interviewSolutionGuid = Guid.Parse(interviewSolutionId);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("interview solution id to be parsed is null");
            }
            catch (FormatException)
            {
                return BadRequest("interview solution id should be in UUID format");
            }
            
            var interviewSolution = await _context.InterviewSolutions.FindAsync(interviewSolutionGuid);
            if (interviewSolution == null)
                return BadRequest("no interview solution with such id");
            
            interviewSolution.EndTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}