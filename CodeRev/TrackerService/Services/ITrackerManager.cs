using TrackerService.Contracts;
using TrackerService.Contracts.Record;

namespace TrackerService.Services;

public interface ITrackerManager
{
    public Task<RecordChunkDto[]?> Get(Guid taskSolutionId, decimal? saveTime);
    public Task<LastCodeDto?> GetLastCode(Guid taskSolutionId);
    public Task Save(TaskRecordDto request);
}