using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
                        TaskOrder = (char)letterOrder++,
                        TaskText = t.TaskText,
                        StartCode = t.StartCode,
                        IsDone = tSln.IsDone
                    })
                .ToList();
            
            return Ok(taskInfos);
        }
    }
}