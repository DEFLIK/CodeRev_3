using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.Core.Models
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