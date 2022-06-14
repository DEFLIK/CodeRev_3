using Bua.CodeRev.TrackerService.Contracts;
using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.Services;

public interface ITrackerManager
{
    public Task<RecordChunkDto[]?> Get(Guid taskSolutionId, decimal? saveTime);
    public Task<LastCodeDto?> GetLastCode(Guid taskSolutionId);
    public Task Save(TaskRecordDto request);
}