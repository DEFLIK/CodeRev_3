using TrackerService.Contracts.Record;

namespace TrackerService.DataAccess.Repositories;

public interface IRepository
{
    public Task<TaskRecordDto?> Get(Guid taskSolutionId);
    public Task Save(TaskRecordDto request);
}