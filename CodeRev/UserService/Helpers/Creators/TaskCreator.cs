using System;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers.Creators
{
    public interface ITaskCreator
    {
        Guid Create();
        Guid CreateSolution(Guid interviewSolutionGuid, Guid taskGuid);
    }
    
    public class TaskCreator : ITaskCreator
    {
        private readonly IDbRepository dbRepository;

        public TaskCreator(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public Guid Create()
        {
            throw new NotImplementedException();
        }

        public Guid CreateSolution(Guid interviewSolutionGuid, Guid taskGuid)
        {
            var taskSolutionGuid = Guid.NewGuid();
            
            dbRepository.Add(new TaskSolution
            {
                Id = taskSolutionGuid,
                InterviewSolutionId = interviewSolutionGuid,
                TaskId = taskGuid,
                IsDone = false,
                Grade = Grade.Zero,
            }).Wait();
            
            return taskSolutionGuid;
        }
    }
}