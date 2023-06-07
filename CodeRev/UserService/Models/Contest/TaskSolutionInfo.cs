using System;
using System.Text.Json.Serialization;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Contest
{
    public class TaskSolutionInfoContest
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid TaskId { get; set; }
        public char TaskOrder { get; set; }
        public string TaskText { get; set; }
        public string StartCode { get; set; }
        public bool IsDone { get; set; }
        public int RunAttemptsLeft { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}