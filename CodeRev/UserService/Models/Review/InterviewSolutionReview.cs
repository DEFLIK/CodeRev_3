using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Review
{
    public class InterviewSolutionReview
    {
        public string UserId { get; set; }
        [Required]
        public string InterviewSolutionId { get; set; }
        [Required]
        public string ReviewerComment { get; set; }
        [Required]
        public GradeEnum AverageGrade { get; set; }
        [Required]
        public InterviewResultEnum InterviewResult { get; set; }
        [Required]
        public IList<TaskSolutionReview> TaskSolutionsReviews { get; set; }
    }
}