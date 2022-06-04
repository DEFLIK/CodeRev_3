using System;

namespace Bua.CodeRev.UserService.Core.Models.Contest
{
    public class TaskSolutionInfoContest
    {
        public Guid Id { get; set; }
        public char TaskOrder { get; set; }
        public string TaskText { get; set; }
        public string StartCode { get; set; }
        public bool IsDone { get; set; }
    }
}