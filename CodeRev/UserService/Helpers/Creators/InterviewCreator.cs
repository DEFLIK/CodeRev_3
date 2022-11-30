using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers.Creators
{
    public interface IInterviewCreator
    {
        Guid Create();
        Guid CreateSolution(Guid userGuid, Guid interviewGuid);
    }

    public class InterviewCreator : IInterviewCreator
    {
        private readonly IDbRepository dbRepository;
        private readonly ITaskCreator taskCreator;
        private readonly IReviewerDraftCreator reviewerDraftCreator;

        public InterviewCreator(IDbRepository dbRepository, ITaskCreator taskCreator, IReviewerDraftCreator reviewerDraftCreator)
        {
            this.dbRepository = dbRepository;
            this.taskCreator = taskCreator;
            this.reviewerDraftCreator = reviewerDraftCreator;
        }

        public Guid Create()
        {
            throw new NotImplementedException();
        }

        public Guid CreateSolution(Guid userGuid, Guid interviewGuid)
        {
            var interviewSolutionGuid = Guid.NewGuid();
            var reviewerDraftId = reviewerDraftCreator.Create(interviewSolutionGuid);
            
            dbRepository.Add(new InterviewSolution
            {
                Id = interviewSolutionGuid,
                UserId = userGuid,
                InterviewId = interviewGuid,
                ReviewerDraftId = reviewerDraftId,
                StartTimeMs = -1,
                EndTimeMs = -1,
                TimeToCheckMs = -1,
                ReviewerComment = "",
                InterviewResult = InterviewResult.NotChecked,
                IsSubmittedByCandidate = false,
            }).Wait();

            foreach (var taskId in dbRepository.Get<InterviewTask>(it => it.InterviewId == interviewGuid).Select(it => it.TaskId))
                taskCreator.CreateSolution(interviewSolutionGuid, taskId);

            return interviewSolutionGuid;
        }
    }
}