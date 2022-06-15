using Bua.CodeRev.TrackerService.Contracts;
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

    public async Task<RecordChunkDto[]?> Get(Guid taskSolutionId, decimal? saveTime)
    {
        var recordsRequest = await repository.Get(taskSolutionId);
        var result = recordsRequest?.RecordChunks.Where(x => x.SaveTime > (saveTime ?? 0m))
                                   .OrderBy(x => x.SaveTime).ToArray();
        return result;
    }

    public async Task<LastCodeDto?> GetLastCode(Guid taskSolutionId)
    {
        var taskRecord = await repository.Get(taskSolutionId);
        return new LastCodeDto {Code = taskRecord?.Code};
    }

    public async Task Save(TaskRecordDto request)
    {
        await repository.Save(request);
    }
}