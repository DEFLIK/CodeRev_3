using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserService.DAL.Models.Draft;
using UserService.DAL.Models.Enums;
using UserService.Helpers;
using UserService.Models.Interviews;
using UserService.Models.Review;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class InterviewsController : Controller
    {
        private const string Solution = "solution";
        
        private readonly IInterviewHelper interviewHelper;
        private readonly IDraftHelper draftHelper;
        
        public InterviewsController(IInterviewHelper interviewHelper, IDraftHelper draftHelper)
        {
            this.interviewHelper = interviewHelper;
            this.draftHelper = draftHelper;
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet]
        public IActionResult GetInterviews()
        {
            return Ok(interviewHelper.GetAllInterviews());
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet("vacancies")]
        public IActionResult GetVacancies()
        {
            return Ok(interviewHelper.GetAllVacancies());
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet($"{Solution}")]
        public IActionResult GetInterviewSolutionInfo([Required] [FromQuery(Name = "id")] string interviewSolutionId)
        {
            var interviewSolutionInfo = interviewHelper.GetInterviewSolutionInfo(interviewSolutionId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (interviewSolutionInfo == null)
                return Conflict("no interview solution with such id, interview or user doesn't exist");
            return Ok(interviewSolutionInfo);
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPut($"{Solution}/grade")]
        public IActionResult PutInterviewSolutionGrade([Required] [FromQuery(Name = "id")] string interviewSolutionId, [Required] [FromQuery(Name = "grade")] int grade)
        {
            if (!Enum.IsDefined(typeof(GradeEnum), grade))
                return BadRequest($"{nameof(grade)} is invalid");
            
            if (!interviewHelper.TryPutInterviewSolutionGrade(interviewSolutionId, (GradeEnum)grade, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }

        [Authorize(Roles = "HrManager,Admin")]
        [HttpPut($"{Solution}/result")]
        public IActionResult PutInterviewSolutionResult([Required] [FromQuery(Name = "id")] string interviewSolutionId, [Required] [FromQuery(Name = "result")] int interviewResult)
        {
            if (!Enum.IsDefined(typeof(InterviewResultEnum), interviewResult))
                return BadRequest($"{nameof(interviewResult)} is invalid");
            
            if (!interviewHelper.TryPutInterviewSolutionResult(interviewSolutionId, (InterviewResultEnum)interviewResult, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPut($"{Solution}/comment")]
        public IActionResult PutInterviewSolutionComment([Required] [FromQuery(Name = "id")] string interviewSolutionId, [Required] [FromBody] InterviewSolutionComment interviewSolutionComment)
        {
            if (!interviewHelper.TryPutInterviewSolutionComment(interviewSolutionId, interviewSolutionComment.ReviewerComment, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }
        
        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPut($"{Solution}/review")]
        public IActionResult PutInterviewSolutionReview([Required] [FromBody] InterviewSolutionReview interviewSolutionReview)
        {
            if (!Enum.IsDefined(typeof(GradeEnum), interviewSolutionReview.AverageGrade))
                return BadRequest($"{nameof(interviewSolutionReview.AverageGrade)} is invalid");
            if (!Enum.IsDefined(typeof(InterviewResultEnum), interviewSolutionReview.InterviewResult))
                return BadRequest($"{nameof(interviewSolutionReview.InterviewResult)} is invalid");
            
            
            if (!interviewHelper.TryPutInterviewSolutionReview(interviewSolutionReview, out var errorString) || errorString != null)
                return BadRequest(errorString);
            return Ok();
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpGet($"{Solution}/draft")]
        public IActionResult GetDraft([Required] [FromQuery(Name = "id")] string interviewSolutionId)
        {
            var interviewSolution = interviewHelper.GetInterviewSolution(interviewSolutionId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (interviewSolution == null)
                return Conflict("no interview solution with such id, interview or user doesn't exist");

            var draft = draftHelper.GetDraft(interviewSolution.ReviewerDraftId);
            return Ok(draft);
        }

        [Authorize(Roles = "Interviewer,HrManager,Admin")]
        [HttpPost($"{Solution}/draft")]
        public IActionResult PostDraft([Required] [FromBody] ReviewerDraftDto reviewerDraft)
        {
            var interviewSolution = interviewHelper.GetInterviewSolution(reviewerDraft.InterviewSolutionId, out var errorString);
            if (errorString != null)
                return BadRequest(errorString);
            if (interviewSolution == null)
                return Conflict("no interview solution with such id, interview or user doesn't exist");

            draftHelper.PutDraft(interviewSolution.ReviewerDraftId, reviewerDraft.Draft);
            return Ok();
        }
    }
}