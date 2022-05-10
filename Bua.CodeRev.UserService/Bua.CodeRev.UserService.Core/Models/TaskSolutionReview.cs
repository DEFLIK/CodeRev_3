using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.Core.Models
{
    public class TaskSolutionReview
    {
        [Required]
        public string TaskSolutionId { get; set; }
        [Required]
        public GradeEnum Grade { get; set; }
    }
}