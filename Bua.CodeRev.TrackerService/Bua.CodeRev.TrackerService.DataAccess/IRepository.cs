using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.DataAccess;

public interface IRepository
{
    public RecordsRequestDto Get(Guid taskSolutionId);
    public void Create(RecordsRequestDto request);
}