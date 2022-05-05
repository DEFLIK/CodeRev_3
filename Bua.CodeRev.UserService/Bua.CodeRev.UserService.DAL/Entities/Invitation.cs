using System;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.DAL.Entities
{
    public class Invitation : BaseEntity
    {
        public RoleEnum Role { get; set; }
        public Guid InterviewId { get; set; }
        public long ExpiredAt { get; set; }
    }
}