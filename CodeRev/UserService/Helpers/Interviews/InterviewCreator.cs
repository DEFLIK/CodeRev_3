using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers.Notifications;
using UserService.Helpers.Tasks;
using UserService.Models.Interviews;

namespace UserService.Helpers.Interviews
{
    public interface IInterviewCreator
    {
        Guid Create(InterviewCreationDto interviewCreation, Guid creatorId);
        Guid CreateSolution(Guid userGuid, Guid interviewGuid, Guid invitingUserId, bool isSynchronous);
    }

    public class InterviewCreator : IInterviewCreator
    {
        private readonly IDbRepository dbRepository;
        private readonly ITaskCreator taskCreator;
        private readonly IReviewerDraftCreator reviewerDraftCreator;
        private readonly INotificationsCreator notificationsCreator;
        private readonly IInterviewHelper interviewHelper;
        private readonly ITaskHandler taskHandler;

        public InterviewCreator(IDbRepository dbRepository, ITaskCreator taskCreator, IReviewerDraftCreator reviewerDraftCreator, INotificationsCreator notificationsCreator,
            IInterviewHelper interviewHelper, ITaskHandler taskHandler)
        {
            this.dbRepository = dbRepository;
            this.taskCreator = taskCreator;
            this.reviewerDraftCreator = reviewerDraftCreator;
            this.notificationsCreator = notificationsCreator;
            this.interviewHelper = interviewHelper;
            this.taskHandler = taskHandler;
        }

        public Guid Create(InterviewCreationDto interviewCreation, Guid creatorId)
        {
            var interview = MapInterviewCreationToInterviewEntity(interviewCreation, creatorId);
            interview.Id = Guid.NewGuid();

            dbRepository.Add(interview).Wait();
            interviewCreation.TaskIds.ForEach(taskId =>
            {
                CreateLinkToTask(interview.Id, taskId);
                CreateLinkToLanguage(interview.Id, taskHandler.GetTask(taskId).ProgrammingLanguage);
            });
            
            dbRepository.SaveChangesAsync().Wait();

            return interview.Id;
        }

        public Guid CreateSolution(Guid userGuid, Guid interviewGuid, Guid invitingUserId, bool isSynchronous)
        {
            var interviewSolutionGuid = Guid.NewGuid();
            var reviewerDraftId = reviewerDraftCreator.Create(interviewSolutionGuid);
            var interview = interviewHelper.GetInterview(interviewGuid);
            
            dbRepository.Add(new InterviewSolution
            {
                Id = interviewSolutionGuid,
                UserId = userGuid,
                InterviewId = interviewGuid,
                ReviewerDraftId = reviewerDraftId,
                StartTimeMs = -1,
                EndTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds() + interview.InterviewDurationMs,
                TimeToCheckMs = -1,
                ReviewerComment = "",
                InterviewResult = InterviewResult.NotChecked,
                IsSubmittedByCandidate = false,
                InvitedBy = invitingUserId,
                IsSynchronous = isSynchronous,
            }).Wait();

            var interviewTasks = dbRepository
                .Get<InterviewTask>(it => it.InterviewId == interviewGuid)
                .Select(it => it.TaskId)
                .ToList();
            foreach (var taskId in interviewTasks)
                taskCreator.CreateSolution(interviewSolutionGuid, taskId);

            dbRepository.SaveChangesAsync().Wait();

            notificationsCreator.Create(userGuid, interviewSolutionGuid, NotificationType.UserCreated); // правильнее перенести в UserCreator
            
            return interviewSolutionGuid;
        }

        private void CreateLinkToTask(Guid interviewId, Guid taskId)
            => dbRepository.Add(new InterviewTask
            {
                Id = Guid.NewGuid(),
                InterviewId = interviewId,
                TaskId = taskId,
            }).Wait(); //не сохраняем изменение в БД, потому что сохраним сразу все изменения в Create

        private void CreateLinkToLanguage(Guid interviewId, ProgrammingLanguage programmingLanguage)
            => dbRepository.Add(new InterviewLanguage
            {
                Id = Guid.NewGuid(),
                InterviewId = interviewId,
                ProgrammingLanguage = programmingLanguage,
            }).Wait(); //не сохраняем изменение в БД, потому что сохраним сразу все изменения в Create

        private static Interview MapInterviewCreationToInterviewEntity(InterviewCreationDto interviewCreation, Guid creatorId)
            => new()
            {
                Vacancy = interviewCreation.Vacancy,
                InterviewText = interviewCreation.InterviewText,
                InterviewDurationMs = interviewCreation.InterviewDurationMs,
                CreatedBy = creatorId,
            };
    }
}