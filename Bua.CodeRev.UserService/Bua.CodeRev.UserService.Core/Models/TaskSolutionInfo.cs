using System;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.Core.Models
{
    public class TaskSolutionInfo
    {
        
        public Guid TaskSolutionId { get; set; }
        public Guid TaskId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public string FullName { get; set; }
        public bool IsDone { get; set; }
        public GradeEnum Grade { get; set; }
    }
}