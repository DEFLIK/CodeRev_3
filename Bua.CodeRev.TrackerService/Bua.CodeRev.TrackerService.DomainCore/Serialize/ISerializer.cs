using Bua.CodeRev.TrackerService.Contracts.Record;

namespace Bua.CodeRev.TrackerService.DomainCore.Serialize;

public interface ISerializer
{
    public RecordChunkResponseDto[] Serialize(RecordChunkDto[]? requestDto);
}