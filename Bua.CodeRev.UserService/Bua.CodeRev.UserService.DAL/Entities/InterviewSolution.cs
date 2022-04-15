using System;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.DAL.Entities
{
    public class InterviewSolution : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid InterviewId { get; set; }
        public long StartTimeMillis { get; set; }
        public long EndTimeMillis { get; set; }
        public string ReviewerComment { get; set; }
        public InterviewResultEnum InterviewResult { get; set; }
    }
}