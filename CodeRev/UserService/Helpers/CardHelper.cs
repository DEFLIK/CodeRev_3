using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Review;

namespace UserService.Helpers
{
    public interface ICardHelper
    {
        List<CardInfo> GetCards();
    }
    
    public class CardHelper : ICardHelper
    {
        private readonly IDbRepository dbRepository;

        public CardHelper(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public List<CardInfo> GetCards()
        {
            //todo refactor + optimize
            var groups = dbRepository.Get<TaskSolution>().ToList()
                .GroupBy(t => t.InterviewSolutionId)
                .ToList();
            var cardsInfo = dbRepository.Get<InterviewSolution>().ToList().Join(dbRepository.Get<Interview>().ToList(),
                s => s.InterviewId,
                i => i.Id,
                (s, i) => new CardInfo
                {
                    UserId = s.UserId,
                    InterviewSolutionId = s.Id,
                    Vacancy = i.Vacancy,
                    StartTimeMs = s.StartTimeMs,
                    TimeToCheckMs = s.TimeToCheckMs,
                    ReviewerComment = s.ReviewerComment,
                    AverageGrade = s.AverageGrade,
                    InterviewResult = s.InterviewResult
                }).ToList();
            cardsInfo = cardsInfo.Join(dbRepository.Get<User>().ToList(), 
                c => c.UserId, 
                u => u.Id,
                (c, u) =>
                {
                    c.FullName = u.FullName;
                    return c;
                }).ToList();
            cardsInfo = cardsInfo.Join(groups, 
                c => c.InterviewSolutionId,
                g => g.Key,
                (c, g) => 
                {
                    c.DoneTasksCount = g.Count(t => t.IsDone);
                    c.TasksCount = g.Count();
                    return c;
                }).ToList();
            
            return cardsInfo;
        }
    }
}