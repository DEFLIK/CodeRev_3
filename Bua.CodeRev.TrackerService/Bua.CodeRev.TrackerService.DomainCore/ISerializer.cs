using Bua.CodeRev.TrackerService.Contracts.Record;
using Newtonsoft.Json.Linq;

namespace Bua.CodeRev.TrackerService.DomainCore;

public interface ISerializer
{
    public JArray Serialize(RecordsRequestDto requestDto);
}