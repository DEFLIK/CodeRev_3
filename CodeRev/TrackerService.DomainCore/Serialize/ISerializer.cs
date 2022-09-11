using TrackerService.Contracts.Record;

namespace TrackerService.DomainCore.Serialize;

public interface ISerializer
{
    public RecordChunkResponseDto[] Serialize(RecordChunkDto[]? requestDto);
}