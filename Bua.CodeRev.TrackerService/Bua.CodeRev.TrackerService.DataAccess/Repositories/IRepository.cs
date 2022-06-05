using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.DataAccess.Repositories;

public interface IRepository
{
    public TaskRecordDto? Get(Guid taskSolutionId);
    public void Save(TaskRecordDto? request);
}