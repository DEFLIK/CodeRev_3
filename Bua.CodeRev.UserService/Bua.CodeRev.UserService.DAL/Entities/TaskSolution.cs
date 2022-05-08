using System;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.DAL.Entities
{
    public class TaskSolution : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public bool IsDone { get; set; }
        public GradeEnum Grade { get; set; }
        
    }
}