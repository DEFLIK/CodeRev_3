using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.UserService.DAL.Entities;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.Core.Models
{
    public class InterviewSolutionInfo
    {
        public Guid InterviewSolutionId { get; set; }
        public Guid UserId { get; set; }
        public Guid InterviewId { get; set; }
        public string FullName { get; set; }
        public string Vacancy { get; set; }
        public long StartTimeMs { get; set; }
        public long EndTimeMs { get; set; }
        public long TimeToCheckMs { get; set; } // fixed time until which solution must be checked
        public string ReviewerComment { get; set; }
        public GradeEnum AverageGrade { get; set; }
        public InterviewResultEnum InterviewResult { get; set; }
        public IList<TaskSolutionInfo> TaskSolutionsInfos { get; set; }
    }
}