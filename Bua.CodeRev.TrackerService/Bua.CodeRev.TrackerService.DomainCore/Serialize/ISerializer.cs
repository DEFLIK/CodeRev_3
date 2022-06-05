using Bua.CodeRev.TrackerService.Contracts.Record;
using Newtonsoft.Json.Linq;

namespace Bua.CodeRev.TrackerService.DomainCore.Serialize;

public interface ISerializer
{
    public JArray? Serialize(RecordChunkDto[]? requestDto);
}