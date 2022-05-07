using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.Models;
using Bua.CodeRev.UserService.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
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
        [HttpGet("end-interview-sln")] //todo set timetocheckms parameter
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
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet("get-cards")]
        public IActionResult GetInterviewSolutions()
        {
            var groups = _context.TaskSolutions.ToList().GroupBy(t => t.InterviewSolutionId).ToList();
            var cardsInfo = _context.InterviewSolutions.ToList().Join(_context.Interviews.ToList(),
                s => s.InterviewId,
                i => i.Id,
                (s, i) => new CardInfo
                {
                    UserId = s.UserId,
                    InterviewSolutionId = s.Id,
                    Vacancy = i.Vacancy,
                    StartTimeMs = s.StartTimeMs,
                    TimeToCheckMs = s.TimeToCheckMs,
                    ReviewerComment = s.ReviewerComment,
                    InterviewResult = s.InterviewResult
                }).ToList();
            cardsInfo = cardsInfo.Join(_context.Users.ToList(), 
                c => c.UserId, 
                u => u.Id,
                (c, u) =>
                {
                    c.FullName = u.FullName;
                    return c;
                }).ToList();
            cardsInfo = cardsInfo.Join(groups, 
                c => c.InterviewSolutionId,
                g => g.Key,
                (c, g) => 
                {
                    c.AverageGrade = (float) Math.Round(g.Average(t => (int) t.TaskGrade), 1);
                    c.DoneTasksCount = g.Count(t => t.IsDone);
                    c.TasksCount = g.Count();
                    return c;
                }).ToList();
            
            return Ok(cardsInfo);
        }
    }
}