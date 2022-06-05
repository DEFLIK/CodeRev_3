using System;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.Core.Models.Review
{
    public class CardInfo
    {
        public Guid UserId { get; set; }
        public Guid InterviewSolutionId { get; set; }
        public string FullName { get; set; }
        public string Vacancy { get; set; }
        public long StartTimeMs { get; set; }
        public long TimeToCheckMs { get; set; }
        public GradeEnum AverageGrade { get; set; }
        public string ReviewerComment { get; set; }
        public int DoneTasksCount { get; set; }
        public int TasksCount { get; set; }
        public InterviewResultEnum InterviewResult { get; set; }
    }
}