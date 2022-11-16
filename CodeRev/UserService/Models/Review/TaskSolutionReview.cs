using System.ComponentModel.DataAnnotations;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Review
{
    public class TaskSolutionReview
    {
        [Required]
        public string TaskSolutionId { get; set; }
        [Required]
        public Grade Grade { get; set; }
    }
}