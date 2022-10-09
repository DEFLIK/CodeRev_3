using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Review
{
    public class InterviewSolutionComment
    {
        [Required]
        public string ReviewerComment { get; set; }
    }
}