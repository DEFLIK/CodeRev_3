using System;
using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Review;

namespace UserService.Helpers
{
    public interface IInterviewHelper
    {
        IEnumerable<Interview> GetAllInterviews();
        IEnumerable<string> GetAllVacancies();
        string GetVacancy(Guid interviewId);
        bool TryPutInterviewSolutionGrade(string interviewSolutionId, GradeEnum grade, out string errorString);
        bool TryPutInterviewSolutionResult(string interviewSolutionId, InterviewResultEnum interviewResult, out string errorString);
        bool TryPutInterviewSolutionComment(string interviewSolutionId, string reviewerComment, out string errorString);
        bool TryPutInterviewSolutionReview(InterviewSolutionReview interviewSolutionReview, out string errorString);
        Interview GetInterview(Guid interviewId);
        InterviewSolution GetInterviewSolution(Guid interviewSolutionId);
        InterviewSolutionInfo GetInterviewSolutionInfo(string interviewSolutionId, out string errorString);
    }
    
    public class InterviewHelper : IInterviewHelper
    {
        private readonly IDbRepository dbRepository;
        private readonly IUserHelper userHelper;
        private readonly ITaskHelper taskHelper;

        public InterviewHelper(IDbRepository dbRepository, IUserHelper userHelper, ITaskHelper taskHelper)
        {
            this.dbRepository = dbRepository;
            this.userHelper = userHelper;
            this.taskHelper = taskHelper;
        }

        public IEnumerable<Interview> GetAllInterviews()
            => dbRepository.Get<Interview>();

        public IEnumerable<string> GetAllVacancies()
            => dbRepository.Get<Interview>().GroupBy(interview => interview.Vacancy).Select(g => g.Key);

        public string GetVacancy(Guid interviewId)
            => GetInterview(interviewId)?.Vacancy;

        public bool TryPutInterviewSolutionGrade(string interviewSolutionId, GradeEnum grade, out string errorString)
        {
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return false;
            var interviewSolution = GetInterviewSolution(interviewSolutionGuid);
            if (interviewSolution == null)
            {
                errorString = $"no {nameof(interviewSolution)} with such id";
                return false;
            }

            interviewSolution.AverageGrade = grade;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }

        public bool TryPutInterviewSolutionResult(string interviewSolutionId, InterviewResultEnum interviewResult, out string errorString)
        {
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return false;
            var interviewSolution = GetInterviewSolution(interviewSolutionGuid);
            if (interviewSolution == null)
            {
                errorString = $"no {nameof(interviewSolution)} with such id";
                return false;
            }

            interviewSolution.InterviewResult = interviewResult;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }

        public bool TryPutInterviewSolutionComment(string interviewSolutionId, string reviewerComment, out string errorString)
        {
            if (reviewerComment == null)
            {
                errorString = $"{nameof(reviewerComment)} can't be null";
                return false;
            }
            
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return false;
            var interviewSolution = GetInterviewSolution(interviewSolutionGuid);
            if (interviewSolution == null)
            {
                errorString = $"no {nameof(interviewSolution)} with such id";
                return false;
            }

            interviewSolution.ReviewerComment = reviewerComment;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }

        public bool TryPutInterviewSolutionReview(InterviewSolutionReview interviewSolutionReview, out string errorString)
        {
            if (interviewSolutionReview.ReviewerComment == null)
            {
                errorString = $"{nameof(interviewSolutionReview.ReviewerComment)} can't be null";
                return false;
            }
            var interviewSolutionId = interviewSolutionReview.InterviewSolutionId;
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return false;
            var interviewSolution = GetInterviewSolution(interviewSolutionGuid);
            if (interviewSolution == null)
            {
                errorString = $"no {nameof(interviewSolution)} with such id";
                return false;
            }
            
            foreach (var taskSolutionReview in interviewSolutionReview.TaskSolutionsReviews)
            {
                if (!taskHelper.TryPutTaskSolutionGrade(taskSolutionReview.TaskSolutionId, taskSolutionReview.Grade, out errorString) || errorString != null)
                    return false;
            }
            interviewSolution.ReviewerComment = interviewSolutionReview.ReviewerComment;
            interviewSolution.AverageGrade = interviewSolutionReview.AverageGrade;
            interviewSolution.InterviewResult = interviewSolutionReview.InterviewResult;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }

        public Interview GetInterview(Guid interviewId)
            => dbRepository
                .Get<Interview>(i => i.Id == interviewId)
                .FirstOrDefault();

        public InterviewSolution GetInterviewSolution(Guid interviewSolutionId)
            => dbRepository
                .Get<InterviewSolution>(i => i.Id == interviewSolutionId)
                .FirstOrDefault();

        public InterviewSolutionInfo GetInterviewSolutionInfo(string interviewSolutionId, out string errorString)
        {
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return null;

            var interviewSolution = GetInterviewSolution(interviewSolutionGuid);
            if (interviewSolution == null)
                return null;

            var vacancy = GetVacancy(interviewSolution.InterviewId);
            if (vacancy == null)
                return null;

            var userFullName = userHelper.GetFullName(interviewSolution.UserId);
            if (userFullName == null)
                return null;

            var interviewSolutionInfo = new InterviewSolutionInfo
            {
                UserId = interviewSolution.UserId,
                InterviewSolutionId = interviewSolution.Id,
                InterviewId = interviewSolution.InterviewId,
                FullName = userFullName,
                Vacancy = vacancy,
                StartTimeMs = interviewSolution.StartTimeMs,
                EndTimeMs = interviewSolution.EndTimeMs,
                TimeToCheckMs = interviewSolution.TimeToCheckMs,
                ReviewerComment = interviewSolution.ReviewerComment,
                AverageGrade = interviewSolution.AverageGrade,
                InterviewResult = interviewSolution.InterviewResult,
            };
            
            var taskSolutionsInfos = new List<TaskSolutionInfo>();
            foreach (var taskSolution in taskHelper.GetTaskSolutions(interviewSolution.Id))
                taskSolutionsInfos.Add(taskHelper.GetTaskSolutionInfo(taskSolution.Id));
            
            var letterOrder = (int)'A';
            interviewSolutionInfo.TaskSolutionsInfos = taskSolutionsInfos
                .OrderBy(t => t.TaskId)
                .Select(t =>
                {
                    t.TaskOrder = (char) letterOrder++;
                    return t;
                })
                .ToList();
            
            return interviewSolutionInfo;
        }
    }
}