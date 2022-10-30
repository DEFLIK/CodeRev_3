using System.ComponentModel.DataAnnotations;
using UserService.DAL.Models.Draft;

namespace UserService.Models.Interviews
{
    public class ReviewerDraftDto
    {
        [Required]
        public string InterviewSolutionId { get; set; }
        [Required]
        public string Draft { get; set; }
    }
}