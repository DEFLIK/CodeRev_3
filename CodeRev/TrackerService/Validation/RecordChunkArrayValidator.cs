using System.ComponentModel.DataAnnotations;
using TrackerService.Contracts.Record;
using TrackerService.Primitives;

namespace TrackerService.Validation;

public static class RecordChunkArrayValidator
{
    public static void Validate(RecordChunkDto[] recordChunks)
    {
        foreach (var recordChunk in recordChunks)
        {
            if (!Ensure.NotNull(recordChunk))
                throw new ValidationException($"Is null {nameof(recordChunk)}");
            RecordChunkValidator.Validate(recordChunk);
        }
    }
}