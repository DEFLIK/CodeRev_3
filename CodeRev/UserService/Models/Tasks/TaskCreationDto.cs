using System.ComponentModel.DataAnnotations;

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
    }
}