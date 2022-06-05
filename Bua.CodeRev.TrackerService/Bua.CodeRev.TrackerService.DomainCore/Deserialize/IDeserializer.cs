using System.Text.Json;
using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.DomainCore.Deserialize;

public interface IDeserializer
{
    public TaskRecordDto ParseRequestDto(JsonElement request);
}