using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Review;

namespace UserService.Helpers.Interviews
{
    public interface ICardHelper
    {
        List<CardInfo> GetCards();
    }
    
    public class CardHelper : ICardHelper
    {
        private readonly IDbRepository dbRepository;
        private readonly IStatusChecker statusChecker;

        public CardHelper(IDbRepository dbRepository, IStatusChecker statusChecker)
        {
            this.dbRepository = dbRepository;
            this.statusChecker = statusChecker;
        }

        public List<CardInfo> GetCards()
        {
            //todo refactor + optimize
            var taskSolutionsByInterviewSolutionsGroups = dbRepository.Get<TaskSolution>()
                .ToList()
                .GroupBy(taskSolution => taskSolution.InterviewSolutionId)
                .ToList();
            var cardsInfo = dbRepository.Get<InterviewSolution>()
                .ToList()
                .Join(dbRepository.Get<Interview>()
                    .ToList(),
                interviewSolution => interviewSolution.InterviewId,
                interview => interview.Id,
                (interviewSolution, interview) => new CardInfo
                {
                    UserId = interviewSolution.UserId,
                    InterviewSolutionId = interviewSolution.Id,
                    Vacancy = interview.Vacancy,
                    StartTimeMs = interviewSolution.StartTimeMs,
                    EndTimeMs = interviewSolution.EndTimeMs,
                    TimeToCheckMs = interviewSolution.TimeToCheckMs,
                    ReviewerComment = interviewSolution.ReviewerComment,
                    AverageGrade = interviewSolution.AverageGrade,
                    InterviewResult = interviewSolution.InterviewResult,
                    IsSubmittedByCandidate = interviewSolution.IsSubmittedByCandidate,
                    IsSolutionTimeExpired = statusChecker.IsSolutionTimeExpired(interviewSolution.EndTimeMs),
                    HasReviewerCheckResult = statusChecker.HasReviewerCheckResult(interviewSolution.AverageGrade),
                    HasHrCheckResult = statusChecker.HasHrCheckResult(interviewSolution.InterviewResult),
                    ProgrammingLanguage = interview.ProgrammingLanguage,
                    IsSynchronous = interview.IsSynchronous,
                })
                .Where(card => !card.IsSynchronous || card.IsSubmittedByCandidate)
                .ToList();
            
            cardsInfo = cardsInfo.Join(
                dbRepository.Get<User>()
                    .ToList(), 
                card => card.UserId, 
                user => user.Id,
                (card, user) =>
                {
                    var splitFullName = user.FullName.Split(' ');
                    card.FirstName = splitFullName.FirstOrDefault();
                    card.Surname = splitFullName.ElementAtOrDefault(1);
                    return card;
                })
                .ToList();
            
            cardsInfo = cardsInfo.Join(
                taskSolutionsByInterviewSolutionsGroups, 
                card => card.InterviewSolutionId,
                group => group.Key,
                (card, group) => 
                {
                    card.DoneTasksCount = group.Count(t => t.IsDone);
                    card.TasksCount = group.Count();
                    return card;
                })
                .ToList();
            
            return cardsInfo;
        }
    }
}