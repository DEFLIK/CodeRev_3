using TrackerService.Contracts.Record;

namespace TrackerService.Infrastructure.Deserialize;

public interface IDeserializer
{
    public TaskRecordDto ParseRequestDto(TaskRecordRequestDto request);
}