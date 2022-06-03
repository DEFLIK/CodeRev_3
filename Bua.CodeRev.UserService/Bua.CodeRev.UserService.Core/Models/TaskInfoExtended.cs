using System;

namespace Bua.CodeRev.UserService.Core.Models
{
    public class TaskInfoExtended
    {
        public Guid TaskSolutionId { get; set; }
        public Guid TaskId { get; set; }
        public char TaskOrder;
        public string TaskText { get; set; }
        public string StartCode { get; set; }
        public bool IsDone { get; set; }
    }
}