using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.LogicHelpers;
using Bua.CodeRev.UserService.Core.Models.Contest;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class ContestController : ParentController
    {
        private const long TimeToCheckInterviewSolutionMs = 604800000; // == 1 week //todo make config setting
        
        public ContestController(IDbRepository dbRepository) : base(dbRepository)
        {
        }
        
        //[Authorize]
        [HttpGet("i-sln-info")]
        public async Task<IActionResult> GetInterviewSolutionInfo([Required][FromHeader(Name = "Authorization")] string authorization)
        {
            if (!authorization.StartsWith("Bearer"))
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            var splitValue = authorization.Split();
            if (splitValue.Length != 2)
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            
            var userId = new TokenHelper().TakeUserIdFromToken(splitValue[1]);
            if (userId == null)
                return BadRequest($"Unexpected {nameof(authorization)} header value");
            
            var (userGuid, errorString) = TryParseGuid(userId, nameof(userId));
            if (errorString != null)
                return BadRequest(errorString);
            
            var user = await _dbRepository
                .Get<User>(i => i.Id == userGuid)
                .FirstOrDefaultAsync();
            
            if (user == null)
                return Conflict($"no {nameof(user)} with such id");
            
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(i => i.UserId == user.Id)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");
            
            var interview = await _dbRepository
                .Get<Interview>(i => i.Id == interviewSolution.InterviewId)
                .FirstOrDefaultAsync();
            
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
        
        //[Authorize]
        [HttpPut("start-i-sln")]
        public async Task<IActionResult> StartInterviewSolutionAsync([Required][FromQuery(Name = "id")] string interviewSolutionId)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(i => i.Id == interviewSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");

            if (interviewSolution.StartTimeMs >= 0)
                return Conflict($"{nameof(interviewSolution)} is already started");
                
            var interview = await _dbRepository
                .Get<Interview>(iv => iv.Id == interviewSolution.InterviewId)
                .FirstOrDefaultAsync();
                
            if (interview == null)
                return Conflict($"no {nameof(interview)} with such id");
            
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            interviewSolution.StartTimeMs = nowTime;
            interviewSolution.EndTimeMs = nowTime + interview.InterviewDurationMs;
            interviewSolution.TimeToCheckMs = nowTime + TimeToCheckInterviewSolutionMs;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }
        
        //[Authorize]
        [HttpPut("end-i-sln")]
        public async Task<IActionResult> EndInterviewSolutionAsync([Required][FromQuery(Name = "id")] string interviewSolutionId)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(i => i.Id == interviewSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");
            
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            if (nowTime > interviewSolution.EndTimeMs)
                return Conflict($"{nameof(interviewSolution)} is already end (end time is less than now time) or wasn't started");

            var interview = await _dbRepository
                .Get<Interview>(i => i.Id == interviewSolution.InterviewId)
                .FirstOrDefaultAsync();
            
            if (interview == null)
                return Conflict($"no {nameof(interview)} with such id");
            
            interviewSolution.EndTimeMs = nowTime;
            interviewSolution.TimeToCheckMs = nowTime + TimeToCheckInterviewSolutionMs;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }
        
        //[Authorize]
        [HttpPut("end-task-sln")]
        public async Task<IActionResult> EndTaskSolutionAsync([Required][FromQuery(Name = "id")] string taskSolutionId)
        {
            var (taskSolutionGuid, errorString) = TryParseGuid(taskSolutionId, nameof(taskSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            var taskSolution = await _dbRepository
                .Get<TaskSolution>(t => t.Id == taskSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (taskSolution == null)
                return Conflict($"no {nameof(taskSolution)} with such id");
            if (taskSolution.IsDone)
                return Conflict($"{nameof(taskSolution)} is already done");

            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(i => i.Id == taskSolution.InterviewSolutionId)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");
            
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() > interviewSolution.EndTimeMs)
                return Conflict($"{nameof(interviewSolution)} is already end (end time is less than now time) or wasn't started");

            taskSolution.IsDone = true;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }

        //[Authorize]
        [HttpGet("task-slns-info")]
        public async Task<IActionResult> GetTaskSolutionsInfoAsync([Required] [FromQuery(Name = "id")] string interviewSolutionId)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);

            var letterOrder = (int)'A';
            var taskInfos = (await _dbRepository
                    .Get<TaskSolution>(t => t.InterviewSolutionId == interviewSolutionGuid)
                    .ToListAsync())
                .Join(_dbRepository.Get<DAL.Entities.Task>(),
                    tSln => tSln.TaskId,
                    t => t.Id,
                    (tSln, t) => new TaskSolutionInfoContest
                    {
                        Id = tSln.Id,
                        TaskId = t.Id,
                        TaskOrder = (char)letterOrder++,
                        TaskText = t.TaskText,
                        StartCode = t.StartCode,
                        IsDone = tSln.IsDone
                    })
                .OrderBy(t => t.TaskId)
                .ToList();
            
            return Ok(taskInfos);
        }
    }
}