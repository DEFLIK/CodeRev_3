using TrackerService.Contracts.Record;

namespace TrackerService.Infrastructure.Serialize;

public interface ISerializer
{
    public RecordChunkResponseDto[] Serialize(RecordChunkDto[]? requestDto);
}