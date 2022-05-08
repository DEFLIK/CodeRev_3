using System;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.DAL.Entities
{
    public class InterviewSolution : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid InterviewId { get; set; }
        public long StartTimeMs { get; set; }
        public long EndTimeMs { get; set; }
        public long TimeToCheckMs { get; set; } // fixed time until which solution must be checked
        public string ReviewerComment { get; set; }
        public GradeEnum AverageGrade { get; set; }
        public InterviewResultEnum InterviewResult { get; set; }
    }
}