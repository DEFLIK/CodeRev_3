using System.ComponentModel.DataAnnotations;

namespace Bua.CodeRev.UserService.Core.Models.Review
{
    public class InterviewSolutionComment
    {
        [Required]
        public string ReviewerComment { get; set; }
    }
}