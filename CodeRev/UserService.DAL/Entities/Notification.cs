using System;
using UserService.DAL.Models.Enums;

namespace UserService.DAL.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public NotificationType NotificationType { get; set; }
        public long CreationTimeMs { get; set; } 
    }
}