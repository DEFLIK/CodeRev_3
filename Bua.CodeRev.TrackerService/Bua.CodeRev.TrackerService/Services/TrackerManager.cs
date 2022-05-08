using Bua.CodeRev.TrackerService.Contracts.Record;
using DataAccess;

namespace Bua.CodeRev.TrackerService.Services;

public class TrackerManager: ITrackerManager
{
    private readonly IRepository repository;

    public TrackerManager(IRepository repository)
    {
        this.repository = repository;
    }
    public RecordsRequestDto Get(Guid taskSolutionId)
    {
        return repository.Get(taskSolutionId);
    }

    public void Save(RecordsRequestDto request)
    {
        repository.Create(request);
    }
}