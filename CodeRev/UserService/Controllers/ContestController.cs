using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers;
using UserService.Models.Contest;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class ContestController : Controller
    {
        private readonly ITokenHelper tokenHelper;
        private readonly IUserHelper userHelper;
        private readonly IInterviewHelper interviewHelper;
        private readonly ITaskHelper taskHelper;

        public ContestController(ITokenHelper tokenHelper, IUserHelper userHelper, IInterviewHelper interviewHelper, ITaskHelper taskHelper)
        {
            this.tokenHelper = tokenHelper;
            this.userHelper = userHelper;
            this.interviewHelper = interviewHelper;
            this.taskHelper = taskHelper;
        }
        
        [Authorize]
        [HttpGet("i-sln-info")]
        public IActionResult GetInterviewSolutionInfo([Required][FromHeader(Name = "Authorization")] string authorization)
        {
            if (!authorization.StartsWith("Bearer"))
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            var splitValue = authorization.Split();
            if (splitValue.Length != 2)
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            
            var userId = tokenHelper.TakeUserIdFromToken(splitValue[1]);
            if (userId == null)
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            
            var user = userHelper.Get(userId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (user == null)
                return Conflict($"no {nameof(user)} with such id");

            var interviewSolution = interviewHelper.GetInterviewSolutionByUserId(user.Id);
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");

            var interview = interviewHelper.GetInterview(interviewSolution.InterviewId);
            if (interview == null)
                return Conflict($"no {nameof(interview)} with such id");

            return Ok(new InterviewSolutionInfo
            {
                Id = interviewSolution.Id,
                Vacancy = interview.Vacancy,
                InterviewText = interview.InterviewText,
                InterviewDurationMs = interview.InterviewDurationMs,
                StartTimeMs = interviewSolution.StartTimeMs,
                EndTimeMs = interviewSolution.EndTimeMs,
                IsStarted = (interviewSolution.StartTimeMs >= 0)
            });
        }
        
        [Authorize]
        [HttpPut("start-i-sln")]
        public IActionResult StartInterviewSolution([Required][FromQuery(Name = "id")] string interviewSolutionId)
        {
            if (!interviewHelper.StartInterviewSolution(interviewSolutionId, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }
        
        [Authorize]
        [HttpPut("end-i-sln")]
        public IActionResult EndInterviewSolution([Required][FromQuery(Name = "id")] string interviewSolutionId)
        {
            if (!interviewHelper.EndInterviewSolution(interviewSolutionId, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }
        
        [Authorize]
        [HttpPut("end-task-sln")]
        public IActionResult EndTaskSolution([Required][FromQuery(Name = "id")] string taskSolutionId)
        {
            if (!taskHelper.EndTaskSolution(taskSolutionId, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }

        [Authorize]
        [HttpGet("task-slns-info")]
        public IActionResult GetTaskSolutionsInfo([Required] [FromQuery(Name = "id")] string interviewSolutionId)
        {
            var taskInfos = taskHelper.GetTaskSolutionInfosForContest(interviewSolutionId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (taskInfos == null)
                return BadRequest($"no {nameof(taskInfos)} with such {nameof(interviewSolutionId)}");
            
            return Ok(taskInfos);
        }
    }
}