using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using Task = UserService.DAL.Entities.Task;

namespace UserService.Helpers.Tasks;

public interface ITaskHandler
{
    Task GetTask(Guid taskId);
    TaskSolution GetTaskSolution(Guid taskSolutionId);
}

public class TaskHandler : ITaskHandler
{
    private readonly IDbRepository dbRepository;

    public TaskHandler(IDbRepository dbRepository)
    {
        this.dbRepository = dbRepository;
    }

    public Task GetTask(Guid taskId)
        => dbRepository
          .Get<Task>(t => t.Id == taskId)
          .FirstOrDefault();

    public TaskSolution GetTaskSolution(Guid taskSolutionId)
        => dbRepository
          .Get<TaskSolution>(t => t.Id == taskSolutionId)
          .FirstOrDefault();
}