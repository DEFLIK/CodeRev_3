﻿using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers.Auth;
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
        private readonly IUserHelper userHelper;
        private readonly IInterviewHelper interviewHelper;

        public CardHelper(IDbRepository dbRepository, IStatusChecker statusChecker, IUserHelper userHelper, IInterviewHelper interviewHelper)
        {
            this.dbRepository = dbRepository;
            this.statusChecker = statusChecker;
            this.userHelper = userHelper;
            this.interviewHelper = interviewHelper;
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
                .Join(dbRepository.Get<Interview>().ToList(),
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
                    ProgrammingLanguages = interviewHelper.GetInterviewLanguages(interview.Id),
                    IsSynchronous = interview.IsSynchronous,
                })
                .Where(card => !card.IsSynchronous || card.IsSubmittedByCandidate)
                .ToList();
            
            cardsInfo = cardsInfo.Join(
                dbRepository.Get<User>().ToList(), 
                card => card.UserId, 
                user => user.Id,
                (card, user) =>
                {
                    userHelper.GetFirstNameAndSurname(user, out var firstName, out var surname);
                    card.FirstName = firstName;
                    card.Surname = surname;
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