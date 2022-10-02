using System;
using System.Linq;
using System.Threading.Tasks;
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

        public InterviewCreator(IDbRepository dbRepository, ITaskCreator taskCreator)
        {
            this.dbRepository = dbRepository;
            this.taskCreator = taskCreator;
        }

        public Guid Create()
        {
            throw new NotImplementedException();
        }

        public Guid CreateSolution(Guid userGuid, Guid interviewGuid)
        {
            var interviewSolutionGuid = Guid.NewGuid();
            dbRepository.Add(new InterviewSolution
            {
                Id = interviewSolutionGuid,
                UserId = userGuid,
                InterviewId = interviewGuid,
                StartTimeMs = -1,
                EndTimeMs = -1,
                TimeToCheckMs = -1,
                ReviewerComment = "",
                InterviewResult = InterviewResultEnum.NotChecked
            }).Wait();

            foreach (var taskId in dbRepository.Get<InterviewTask>(it => it.InterviewId == interviewGuid).Select(it => it.TaskId))
                taskCreator.CreateSolution(interviewSolutionGuid, taskId);

            return interviewSolutionGuid;
        }
    }
}