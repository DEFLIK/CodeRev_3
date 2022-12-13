using System;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Tasks;

namespace UserService.Helpers.Tasks
{
    public interface ITaskCreator
    {
        Guid Create(TaskCreationDto taskCreation);
        Guid CreateSolution(Guid interviewSolutionGuid, Guid taskGuid);
    }
    
    public class TaskCreator : ITaskCreator
    {
        private readonly IDbRepository dbRepository;

        public TaskCreator(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public Guid Create(TaskCreationDto taskCreation)
        {
            var task = MapTaskCreationToTaskEntity(taskCreation);
            task.Id = Guid.NewGuid();

            dbRepository.Add(task).Wait();
            
            dbRepository.SaveChangesAsync().Wait();

            return task.Id;
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
            
            dbRepository.SaveChangesAsync().Wait();
            
            return taskSolutionGuid;
        }

        private static Task MapTaskCreationToTaskEntity(TaskCreationDto taskCreation)
            => new()
            {
                TaskText = taskCreation.TaskText,
                StartCode = taskCreation.StartCode,
            };
    }
}