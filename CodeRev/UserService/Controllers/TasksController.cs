using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Enums;
using UserService.Helpers.Tasks;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class TasksController : Controller
    {
        private const string Solution = "solution";

        private readonly ITaskHelper taskHelper;

        public TasksController(ITaskHelper taskHelper)
        {
            this.taskHelper = taskHelper;
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet($"{Solution}")]
        public IActionResult GetTaskSolutionInfo([Required] [FromQuery(Name = "id")] string taskSolutionId)
        {
            var taskSolutionInfo = taskHelper.GetTaskSolutionInfo(taskSolutionId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (taskSolutionInfo == null)
                return BadRequest("no task solution with such id or user solution refers to doesn't exist");
            return Ok(taskSolutionInfo);
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPut($"{Solution}/grade")]
        public IActionResult PutTaskSolutionGrade([Required] [FromQuery(Name = "id")] string taskSolutionId, [Required] [FromQuery(Name = "grade")] int grade)
        {
            if (!Enum.IsDefined(typeof(Grade), grade))
                return BadRequest($"{nameof(grade)} is invalid");
            
            if (!taskHelper.TryPutTaskSolutionGrade(taskSolutionId, (Grade)grade, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }
    }
}