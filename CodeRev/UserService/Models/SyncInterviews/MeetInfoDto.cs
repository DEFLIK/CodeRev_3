using System;
using System.Collections.Generic;
using UserService.DAL.Models.Enums;

namespace UserService.Models.SyncInterviews
{
    public class MeetInfoDto
    {
        public Guid UserId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public Guid InterviewId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Vacancy { get; set; }
        public int TasksCount { get; set; }
        public IList<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public bool IsOwnerMeet { get; set; }
    }
}