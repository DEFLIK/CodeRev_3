using Bua.CodeRev.TrackerService.Contracts.Record;

namespace DataAccess;

public interface IRepository
{
    public RecordsRequestDto Get(Guid taskSolutionId);
    public void Create(RecordsRequestDto request);
}