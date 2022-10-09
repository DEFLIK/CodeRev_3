using System.ComponentModel.DataAnnotations;
using TrackerService.Contracts.Record;
using TrackerService.Primitives;

namespace TrackerService.Validation;

public static class RecordChunkValidator
{
    public static void Validate(RecordChunkDto recordChunk)
    {
        if (!Ensure.GreaterThanOrEqualTo(recordChunk.SaveTime, -1m))
            throw new ValidationException(
                $"Less than or equal to -1.0 {nameof(recordChunk.SaveTime)}: {recordChunk.SaveTime}");
    }
}