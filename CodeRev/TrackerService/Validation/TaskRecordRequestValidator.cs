using System.ComponentModel.DataAnnotations;
using TrackerService.Contracts.Record;
using TrackerService.Primitives;

namespace TrackerService.Validation;

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