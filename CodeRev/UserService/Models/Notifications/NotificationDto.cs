using System;
using System.Collections.Generic;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Notifications
{
    public class NotificationDto
    {
        public Guid UserId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public NotificationType NotificationType { get; set; }
        public long CreationTimeMs { get; set; } 
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Vacancy { get; set; }
        public IList<ProgrammingLanguage> ProgrammingLanguages { get; set; }
    }
}