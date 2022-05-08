using System.Text.Json;
using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.DomainCore;

public interface IParser
{
    public RecordsRequestDto ParseRequestDto(JsonElement request);
}