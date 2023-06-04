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
        private readonly ITaskHelper taskHelper;

        public TaskCreator(IDbRepository dbRepository, ITaskHelper taskHelper)
        {
            this.dbRepository = dbRepository;
            this.taskHelper = taskHelper;
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
            var task = taskHelper.GetTask(taskGuid);
            
            dbRepository.Add(new TaskSolution
            {
                Id = taskSolutionGuid,
                InterviewSolutionId = interviewSolutionGuid,
                TaskId = task.Id,
                IsDone = false,
                Grade = Grade.Zero,
                RunAttemptsLeft = task.RunAttempts,
            }).Wait();
            
            dbRepository.SaveChangesAsync().Wait();
            
            return taskSolutionGuid;
        }

        private static Task MapTaskCreationToTaskEntity(TaskCreationDto taskCreation)
            => new()
            {
                TaskText = taskCreation.TaskText,
                StartCode = taskCreation.StartCode,
                Name = taskCreation.Name,
                RunAttempts = taskCreation.RunAttempts >= 0 ? taskCreation.RunAttempts : 0,
            };
    }
}