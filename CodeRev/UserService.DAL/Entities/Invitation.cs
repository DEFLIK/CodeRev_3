using System;
using UserService.DAL.Models.Enums;

namespace UserService.DAL.Entities
{
    public class Invitation : BaseEntity
    {
        public RoleEnum Role { get; set; }
        public Guid InterviewId { get; set; }
        public long ExpiredAt { get; set; }
    }
}