using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using Task = UserService.DAL.Entities.Task;

namespace TaskTestsProvider;

public interface ITaskTestsProviderClient
{
    string GetTaskTestsCode(Guid taskId);
    string GetTaskTestsCodeBySolutionId(Guid taskSolutionId);
}

public class TaskTestsProviderClient : ITaskTestsProviderClient
{
    private readonly IDbRepository dbRepository;

    public TaskTestsProviderClient(IDbRepository dbRepository)
    {
        this.dbRepository = dbRepository;
    }

    public string GetTaskTestsCode(Guid taskId)
        => GetTask(taskId)?.TestsCode ?? string.Empty;

    public string GetTaskTestsCodeBySolutionId(Guid taskSolutionId)
    {
        var taskId = GetTaskSolution(taskSolutionId)?.TaskId ?? Guid.Empty;
        if (taskId.Equals(Guid.Empty))
            return string.Empty;
        return GetTask(taskId)?.TestsCode ?? string.Empty;
    }

    private TaskSolution? GetTaskSolution(Guid taskSolutionId)
        => dbRepository
          .Get<TaskSolution>(t => t.Id == taskSolutionId)
          .FirstOrDefault();
    
    private Task? GetTask(Guid taskId)
        => dbRepository
          .Get<Task>(t => t.Id == taskId)
          .FirstOrDefault();
}