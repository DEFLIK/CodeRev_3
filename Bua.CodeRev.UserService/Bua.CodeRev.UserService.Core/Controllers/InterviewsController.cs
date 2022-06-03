using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.UserService.Core.Models;
using Bua.CodeRev.UserService.DAL;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models;
using Bua.CodeRev.UserService.DAL.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace Bua.CodeRev.UserService.Core.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class InterviewsController : Controller
    {
        private readonly IDbRepository _dbRepository;
        
        public InterviewsController(IDbRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }
        
        //[Authorize]
        [HttpGet("start-interview-sln")]
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
                
            var interview = await _dbRepository
                .Get<Interview>(iv => iv.Id == interviewSolution.InterviewId)
                .FirstOrDefaultAsync();
                
            if (interview == null)
                return Conflict($"no {nameof(interview)} with such id");
            
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            interviewSolution.StartTimeMs = nowTime;
            interviewSolution.EndTimeMs = nowTime + interview.InterviewDurationMs;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }
        
        //[Authorize]
        [HttpGet("end-interview-sln")] //todo set timetocheckms parameter
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
            
            interviewSolution.EndTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }
        
        //[Authorize]
        [HttpPut("end-task-sln")]
        public async Task<IActionResult> EndTaskSolutionAsync([Required] [FromQuery(Name = "id")] string taskSolutionId)
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

        
        //[Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPut("put-task-sln-grade")]
        public async Task<IActionResult> PutTaskSolutionGradeAsync([Required] [FromQuery(Name = "id")] string taskSolutionId, 
            [Required][FromQuery(Name = "grade")] int grade, string interviewSolutionId)
        {
            if (!Enum.IsDefined(typeof(GradeEnum), grade))
                return BadRequest($"{nameof(grade)} is invalid");
            
            var (taskSolutionGuid, errorString) = TryParseGuid(taskSolutionId, nameof(taskSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            var taskSolution = await _dbRepository
                .Get<TaskSolution>(t => t.Id == taskSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (taskSolution == null)
                return Conflict($"no {nameof(taskSolution)} with such id");
            if (taskSolution.IsDone == false)
                return Conflict($"{nameof(taskSolution)} wasn't done yet");

            if (interviewSolutionId != null)
            {
                var (interviewSolutionGuid, errorStringNew) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
                if (errorStringNew != null)
                    return BadRequest(errorStringNew);
                var interviewSolution = await _dbRepository
                    .Get<InterviewSolution>(t => t.Id == interviewSolutionGuid)
                    .FirstOrDefaultAsync();
                
                if (interviewSolution == null)
                    return Conflict($"no {nameof(interviewSolution)} with such id");
                if (interviewSolution.Id != taskSolution.InterviewSolutionId)
                    return Conflict($"{nameof(interviewSolutionGuid)} doesn't match {nameof(taskSolution)}.{nameof(taskSolution.InterviewSolutionId)}");
            }

            taskSolution.Grade = (GradeEnum) grade;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }

        //[Authorize(Roles = "HrManager,Admin")]
        [HttpPut("put-i-sln-result")]
        public async Task<IActionResult> PutInterviewSolutionResultAsync([Required] [FromQuery(Name = "id")] string interviewSolutionId,
            [Required] [FromQuery(Name = "result")] int interviewResult)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(t => t.Id == interviewSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");
            if (!Enum.IsDefined(typeof(InterviewResultEnum), interviewResult))
                return BadRequest($"{nameof(interviewResult)} is invalid");
            if (interviewSolution.InterviewResult != InterviewResultEnum.NotChecked)
                return Conflict($"{nameof(interviewResult)} is already put");
            
            interviewSolution.InterviewResult = (InterviewResultEnum) interviewResult;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }

        //[Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPut("put-i-sln-review")]
        public async Task<IActionResult> PutInterviewSolutionReviewAsync([Required] [FromBody] InterviewSolutionReview interviewSolutionReview)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionReview.InterviewSolutionId, 
                nameof(interviewSolutionReview.InterviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(t => t.Id == interviewSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return Conflict($"no {nameof(interviewSolution)} with such id");
            if (interviewSolutionReview.ReviewerComment == null)
                return Conflict($"{nameof(interviewSolutionReview.ReviewerComment)} can't be null");
            if (!Enum.IsDefined(typeof(GradeEnum), interviewSolutionReview.AverageGrade))
                return BadRequest($"{nameof(interviewSolutionReview.AverageGrade)} is invalid");
            if (!Enum.IsDefined(typeof(InterviewResultEnum), interviewSolutionReview.InterviewResult))
                return BadRequest($"{nameof(interviewSolutionReview.InterviewResult)} is invalid");
            
            foreach (var taskSolutionReview in interviewSolutionReview.TaskSolutionsReviews)
            {
                var answer = await PutTaskSolutionGradeAsync(taskSolutionReview.TaskSolutionId, 
                    (int) taskSolutionReview.Grade, 
                    interviewSolutionReview.InterviewSolutionId);
                if (!(answer is OkResult))
                    return answer;
            }
            interviewSolution.ReviewerComment = interviewSolutionReview.ReviewerComment;
            interviewSolution.AverageGrade = interviewSolutionReview.AverageGrade;
            interviewSolution.InterviewResult = interviewSolutionReview.InterviewResult;
            await _dbRepository.SaveChangesAsync();
            return Ok();
        }

        //[Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet("i-sln-info")]
        public async Task<IActionResult> GetInterviewSolutionInfoAsync([Required] [FromQuery(Name = "id")] string interviewSolutionId)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);
            
            var interviewSolutionInfo = await GetInterviewSolutionInfoAsync(interviewSolutionGuid);
            if (interviewSolutionInfo == null)
                return Conflict("no interview solution with such id, interview or user doesn't exist");
            return Ok(interviewSolutionInfo);
        }

        //[Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet("task-sln-info")]
        public async Task<IActionResult> GetTaskSolutionInfoAsync([Required] [FromQuery(Name = "id")] string taskSolutionId)
        {
            var (taskSolutionGuid, errorString) = TryParseGuid(taskSolutionId, nameof(taskSolutionId));
            if (errorString != null)
                return BadRequest(errorString);

            var taskSolutionInfo = await GetTaskSolutionInfoAsync(taskSolutionGuid);
            if (taskSolutionInfo == null)
                return Conflict("no task solution with such id or user solution refers to doesn't exist");
            return Ok(taskSolutionInfo);
        }

        //[Authorize]
        [HttpGet("tasks-info")]
        public async Task<IActionResult> GetTasksInfoAsync([Required] [FromQuery(Name = "id")] string interviewSolutionId)
        {
            var (interviewSolutionGuid, errorString) = TryParseGuid(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return BadRequest(errorString);

            var letterOrder = (int)'A';
            var taskInfos = (await _dbRepository
                .Get<TaskSolution>(t => t.InterviewSolutionId == interviewSolutionGuid)
                .ToListAsync())
                .Join(_dbRepository.Get<Bua.CodeRev.UserService.DAL.Entities.Task>(),
                    tSln => tSln.TaskId,
                    t => t.Id,
                    (tSln, t) => new TaskInfoExtended
                    {
                        TaskSolutionId = tSln.Id,
                        TaskId = t.Id,
                        TaskOrder = (char)letterOrder++,
                        TaskText = t.TaskText,
                        StartCode = t.StartCode,
                        IsDone = tSln.IsDone
                    })
                .ToList();
            
            return Ok(taskInfos);
        }
        
        //[Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet("get-cards")]
        public IActionResult GetInterviewSolutions()
        {
            var groups = _dbRepository.Get<TaskSolution>().ToList().GroupBy(t => t.InterviewSolutionId).ToList(); //todo optimize
            var cardsInfo = _dbRepository.Get<InterviewSolution>().ToList().Join(_dbRepository.Get<Interview>().ToList(),
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
                    AverageGrade = s.AverageGrade,
                    InterviewResult = s.InterviewResult
                }).ToList();
            cardsInfo = cardsInfo.Join(_dbRepository.Get<User>().ToList(), 
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
                    c.DoneTasksCount = g.Count(t => t.IsDone);
                    c.TasksCount = g.Count();
                    return c;
                }).ToList();
            
            return Ok(cardsInfo);
        }

        private async Task<InterviewSolutionInfo> GetInterviewSolutionInfoAsync(Guid interviewSolutionGuid)
        {
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(i => i.Id == interviewSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return null;

            var interview = await _dbRepository
                .Get<Interview>(i => i.Id == interviewSolution.InterviewId)
                .FirstOrDefaultAsync();
            
            if (interview == null)
                return null;
            
            var user = await _dbRepository
                .Get<User>(u => u.Id == interviewSolution.UserId)
                .FirstOrDefaultAsync();
            
            if (user == null)
                return null;

            var interviewSolutionInfo = new InterviewSolutionInfo
            {
                UserId = interviewSolution.UserId,
                InterviewSolutionId = interviewSolution.Id,
                InterviewId = interviewSolution.InterviewId,
                FullName = user.FullName,
                Vacancy = interview.Vacancy,
                StartTimeMs = interviewSolution.StartTimeMs,
                EndTimeMs = interviewSolution.EndTimeMs,
                TimeToCheckMs = interviewSolution.TimeToCheckMs,
                ReviewerComment = interviewSolution.ReviewerComment,
                AverageGrade = interviewSolution.AverageGrade,
                InterviewResult = interviewSolution.InterviewResult,
                TaskSolutionsInfos = new List<TaskSolutionInfo>()
            };
            foreach (var taskSolution in _dbRepository
                .Get<TaskSolution>(t => t.InterviewSolutionId == interviewSolution.Id)
                .ToList())
            {
                interviewSolutionInfo.TaskSolutionsInfos.Add(await GetTaskSolutionInfoAsync(taskSolution.Id));
            }

            return interviewSolutionInfo;
        }

        private async Task<TaskSolutionInfo> GetTaskSolutionInfoAsync(Guid taskSolutionGuid)
        {
            var taskSolution = await _dbRepository
                .Get<TaskSolution>(t => t.Id == taskSolutionGuid)
                .FirstOrDefaultAsync();
            
            if (taskSolution == null)
                return null;
            
            var interviewSolution = await _dbRepository
                .Get<InterviewSolution>(i => i.Id == taskSolution.InterviewSolutionId)
                .FirstOrDefaultAsync();
            
            if (interviewSolution == null)
                return null;
            var user = await _dbRepository
                .Get<User>(u => u.Id == interviewSolution.UserId)
                .FirstOrDefaultAsync();
            
            if (user == null)
                return null;
            
            return new TaskSolutionInfo
            {
                TaskSolutionId = taskSolution.Id,
                TaskId = taskSolution.TaskId,
                InterviewSolutionId = taskSolution.InterviewSolutionId,
                FullName = user.FullName,
                Grade = taskSolution.Grade,
                IsDone = taskSolution.IsDone
            };
        }

        private Tuple<Guid, string> TryParseGuid(string id, string nameOfId)
        {
            
            var guid = new Guid();
            string errorString = null;
            try
            {
                guid = Guid.Parse(id);
            }
            catch (ArgumentNullException)
            {
                errorString = $"{nameOfId} to be parsed is null";
            }
            catch (FormatException)
            {
                errorString = $"{nameOfId} should be in UUID format";
            }
            return new Tuple<Guid, string>(guid, errorString);
        }
    }
}