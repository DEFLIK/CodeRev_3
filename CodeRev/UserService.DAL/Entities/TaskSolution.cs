using System;
using UserService.DAL.Models.Enums;

namespace UserService.DAL.Entities
{
    public class TaskSolution : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public bool IsDone { get; set; }
        public GradeEnum Grade { get; set; }
        
    }
}