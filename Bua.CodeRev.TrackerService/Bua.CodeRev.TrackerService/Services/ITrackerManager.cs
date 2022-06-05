using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.Services;

public interface ITrackerManager
{
    public RecordChunkDto[]? Get(Guid taskSolutionId, decimal? saveTime);
    public string? GetLastCode(Guid taskSolutionId);
    public void Save(TaskRecordDto? request);
}