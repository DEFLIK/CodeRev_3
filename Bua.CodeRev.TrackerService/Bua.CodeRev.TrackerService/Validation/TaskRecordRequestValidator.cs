using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.Primitives;

namespace Bua.CodeRev.TrackerService.Validation;

public class TaskRecordRequestValidator
{
    public static void Validate(TaskRecordDto taskRecord)
    {
        if (!Ensure.NotNull(taskRecord))
            throw new ValidationException($"Is null {nameof(taskRecord)}");

        if (!Ensure.NotNull(taskRecord.RecordChunks))
            throw new ValidationException($"Is null {nameof(taskRecord.RecordChunks)}");

        RecordChunkArrayValidator.Validate(taskRecord.RecordChunks);
    }
}