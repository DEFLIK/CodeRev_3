using System;
using UserService.DAL.Models.Enums;

namespace UserService.Helpers.Interviews
{
    public interface IStatusChecker
    {
        bool IsSolutionTimeExpired(long endTimeMs);
        bool HasReviewerCheckResult(Grade grade);
        bool HasHrCheckResult(InterviewResult interviewResult);
    }
    
    public class StatusChecker : IStatusChecker
    {
        public bool IsSolutionTimeExpired(long endTimeMs)
        {
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return endTimeMs < nowTime;
        }

        public bool HasReviewerCheckResult(Grade grade) => grade != Grade.Zero;

        public bool HasHrCheckResult(InterviewResult interviewResult) => interviewResult != InterviewResult.NotChecked;
    }
}