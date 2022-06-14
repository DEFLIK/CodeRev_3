using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.DataAccess.Repositories;

public interface IRepository
{
    public Task<TaskRecordDto?> Get(Guid taskSolutionId);
    public Task Save(TaskRecordDto request);
}