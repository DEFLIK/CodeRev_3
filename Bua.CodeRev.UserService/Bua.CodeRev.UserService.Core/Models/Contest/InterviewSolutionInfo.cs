using System;
using Bua.CodeRev.UserService.DAL.Entities;

namespace Bua.CodeRev.UserService.Core.Models.Contest
{
    public class InterviewSolutionInfo
    {
        public Guid Id { get; set; }
        public string Vacancy { get; set; }
        public string InterviewText { get; set; }
        public long InterviewDurationMs { get; set; }
        public bool IsStarted { get; set; }
    }
}