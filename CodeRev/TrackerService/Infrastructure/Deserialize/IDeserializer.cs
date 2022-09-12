using TrackerService.Contracts.Record;

namespace TrackerService.DomainCore.Deserialize;

public interface IDeserializer
{
    public TaskRecordDto ParseRequestDto(TaskRecordRequestDto request);
}