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
        bool TryPutInterviewSolutionGrade(string interviewSolutionId, Grade grade, out string errorString);
        bool TryPutInterviewSolutionResult(string interviewSolutionId, InterviewResult interviewResult, out string errorString);
        bool TryPutInterviewSolutionComment(string interviewSolutionId, string reviewerComment, out string errorString);
        bool TryPutInterviewSolutionReview(InterviewSolutionReview interviewSolutionReview, out string errorString);
        Interview GetInterview(Guid interviewId);
        InterviewSolution GetInterviewSolutionByUserId(Guid userId);
        InterviewSolution GetInterviewSolution(Guid interviewSolutionId);
        InterviewSolution GetInterviewSolution(string interviewSolutionId, out string errorString);
        InterviewSolutionInfo GetInterviewSolutionInfo(string interviewSolutionId, out string errorString);
        bool StartInterviewSolution(string interviewSolutionId, out string errorString);
        bool EndInterviewSolution(string interviewSolutionId, out string errorString);
    }
    
    public class InterviewHelper : IInterviewHelper
    {
        private const long TimeToCheckInterviewSolutionMs = 604800000; // == 1 week //todo make config setting
        
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

        public bool TryPutInterviewSolutionGrade(string interviewSolutionId, Grade grade, out string errorString)
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

        public bool TryPutInterviewSolutionResult(string interviewSolutionId, InterviewResult interviewResult, out string errorString)
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

        public InterviewSolution GetInterviewSolutionByUserId(Guid userId)
            => dbRepository
                .Get<InterviewSolution>(i => i.UserId == userId)
                .FirstOrDefault();

        public InterviewSolution GetInterviewSolution(Guid interviewSolutionId)
            => dbRepository
                .Get<InterviewSolution>(i => i.Id == interviewSolutionId)
                .FirstOrDefault();

        public InterviewSolution GetInterviewSolution(string interviewSolutionId, out string errorString)
        {
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            return errorString == null ? GetInterviewSolution(interviewSolutionGuid) : null;
        }

        public InterviewSolutionInfo GetInterviewSolutionInfo(string interviewSolutionId, out string errorString)
        {
            (var interviewSolutionGuid, errorString) = GuidParser.TryParse(interviewSolutionId, nameof(interviewSolutionId));
            if (errorString != null)
                return null;

            var interviewSolution = GetInterviewSolution(interviewSolutionGuid);
            if (interviewSolution == null)
                return null;

            var interview = GetInterview(interviewSolution.InterviewId);
            if (interview == null)
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
                Vacancy = interview.Vacancy,
                StartTimeMs = interviewSolution.StartTimeMs,
                EndTimeMs = interviewSolution.EndTimeMs,
                TimeToCheckMs = interviewSolution.TimeToCheckMs,
                ReviewerComment = interviewSolution.ReviewerComment,
                AverageGrade = interviewSolution.AverageGrade,
                InterviewResult = interviewSolution.InterviewResult,
                IsSubmittedByCandidate = interviewSolution.IsSubmittedByCandidate,
                ProgrammingLanguage = interview.ProgrammingLanguage,
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

        public bool StartInterviewSolution(string interviewSolutionId, out string errorString)
        {
            var interviewSolution = GetInterviewSolution(interviewSolutionId, out errorString);
            if (errorString != null)
                return false;
            if (interviewSolution == null)
            {
                errorString = $"no {nameof(interviewSolution)} with such id";
                return false;
            }

            if (interviewSolution.StartTimeMs >= 0)
            {
                errorString = $"{nameof(interviewSolution)} is already started";
                return false;
            }
            
            var interview = GetInterview(interviewSolution.InterviewId);
            if (interview == null)
            {
                errorString = $"no {nameof(interview)} with such id";
                return false;
            }
            
            //todo возможно, надо сделать как в комменте, иначе время не так считаем
            // var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            // var endTime = nowTime + interview.InterviewDurationMs;
            // interviewSolution.StartTimeMs = nowTime;
            // interviewSolution.EndTimeMs = endTime;
            // interviewSolution.TimeToCheckMs = endTime + TimeToCheckInterviewSolutionMs;
            
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            interviewSolution.StartTimeMs = nowTime;
            interviewSolution.EndTimeMs = nowTime + interview.InterviewDurationMs;
            interviewSolution.TimeToCheckMs = nowTime + TimeToCheckInterviewSolutionMs;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }

        public bool EndInterviewSolution(string interviewSolutionId, out string errorString)
        {
            var interviewSolution = GetInterviewSolution(interviewSolutionId, out errorString);
            if (interviewSolution == null)
            {
                errorString = $"no {nameof(interviewSolution)} with such id";
                return false;
            }
            
            var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (nowTime > interviewSolution.EndTimeMs || interviewSolution.IsSubmittedByCandidate)
            {
                errorString = $"{nameof(interviewSolution)} is already ended (end time is less than now time) or wasn't started";
                return false;
            }

            var interview = GetInterview(interviewSolution.InterviewId);
            if (interview == null)
            {
                errorString = $"no {nameof(interview)} with such id";
                return false;
            }
            
            interviewSolution.EndTimeMs = nowTime;
            interviewSolution.TimeToCheckMs = nowTime + TimeToCheckInterviewSolutionMs;
            interviewSolution.IsSubmittedByCandidate = true;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }
    }
}