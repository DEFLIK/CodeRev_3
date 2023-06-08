using System;
using System.Collections.Generic;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Contest
{
    public class InterviewSolutionInfo
    {
        public Guid Id { get; set; }
        public string Vacancy { get; set; }
        public string InterviewText { get; set; }
        public long InterviewDurationMs { get; set; }
        public long StartTimeMs { get; set; }
        public long EndTimeMs { get; set; }
        public bool IsStarted { get; set; }
        public bool IsSubmittedByCandidate { get; set; }
        public IList<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public bool IsSynchronous { get; set; }
    }
}