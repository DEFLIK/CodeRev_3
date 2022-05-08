using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.Services;

public interface ITrackerManager
{
    public RecordsRequestDto Get(Guid taskSolutionId);
    public void Save(RecordsRequestDto request);
}