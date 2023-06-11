using System;
using System.Collections.Generic;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Review
{
    public class InterviewSolutionInfo
    {
        public Guid InterviewSolutionId { get; set; }
        public Guid UserId { get; set; }
        public Guid InterviewId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Vacancy { get; set; }
        public long StartTimeMs { get; set; }
        public long EndTimeMs { get; set; }
        public long TimeToCheckMs { get; set; } // fixed time until which solution must be checked
        public string ReviewerComment { get; set; }
        public Grade AverageGrade { get; set; }
        public InterviewResult InterviewResult { get; set; }
        public bool IsSubmittedByCandidate { get; set; }
        public IList<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public IList<TaskSolutionInfo> TaskSolutionsInfos { get; set; }
    }
}