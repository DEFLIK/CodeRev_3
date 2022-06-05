using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.DataAccess.Repositories;

namespace Bua.CodeRev.TrackerService.Services;

public class TrackerManager : ITrackerManager
{
    private readonly IRepository repository;

    public TrackerManager(IRepository repository)
    {
        this.repository = repository;
    }

    public RecordChunkDto[]? Get(Guid taskSolutionId, decimal? saveTime)
    {
        var recordsRequest = repository.Get(taskSolutionId);
        var result = recordsRequest?.RecordChunks.Where(x => x.SaveTime > (saveTime ?? 0m))
            .OrderBy(x => x.SaveTime).ToArray();
        return result;
    }

    public string? GetLastCode(Guid taskSolutionId)
    {
        var taskRecord = repository.Get(taskSolutionId);
        return taskRecord?.Code;
    }

    public void Save(TaskRecordDto request)
    {
        repository.Save(request);
    }
}