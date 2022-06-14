using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.Primitives;

namespace Bua.CodeRev.TrackerService.Validation;

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