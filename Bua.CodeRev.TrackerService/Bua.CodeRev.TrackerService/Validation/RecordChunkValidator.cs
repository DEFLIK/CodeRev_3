using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.Primitives;

namespace Bua.CodeRev.TrackerService.Validation;

public static class RecordChunkValidator
{
    public static void Validate(RecordChunkDto recordChunk)
    {
        if (!Ensure.GreaterThanOrEqualTo(recordChunk.SaveTime, -1m))
            throw new ValidationException(
                $"Less than or equal to -1.0 {nameof(recordChunk.SaveTime)}: {recordChunk.SaveTime}");
    }
}