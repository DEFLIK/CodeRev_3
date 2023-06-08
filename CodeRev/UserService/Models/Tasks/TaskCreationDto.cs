using System.ComponentModel.DataAnnotations;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Tasks
{
    public class TaskCreationDto
    {
        [Required]
        public string TaskText { get; set; }
        [Required]
        public string StartCode { get; set; }
        [Required]
        public string Name { get; set; }
        public string TestsCode { get; set; }
        [Required]
        public int RunAttempts { get; set; }
        [Required]
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}