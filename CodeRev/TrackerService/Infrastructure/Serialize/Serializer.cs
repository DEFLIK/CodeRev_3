using System.Text.Json;
using System.Text.Json.Nodes;
using TrackerService.Contracts.Actions;
using TrackerService.Contracts.Primitives;
using TrackerService.Contracts.Record;

namespace TrackerService.Infrastructure.Serialize;

public class Serializer : ISerializer
{
    public RecordChunkResponseDto[] Serialize(RecordChunkDto[]? recordChunks)
    {
        if (recordChunks == null)
            return null;
        var result = new List<RecordChunkResponseDto>();
        foreach (var recordChunk in recordChunks)
            result.Add(new RecordChunkResponseDto
            {
                SaveTime = recordChunk.SaveTime,
                Code = recordChunk.Code,
                Records = recordChunk.Records.Select(SerializeRecord).ToArray()
            });

        return result.ToArray();
    }

    private static JsonValue? SerializePeriod(PeriodDto periodDto)
    {
        var from = new[] {periodDto.From.LineNumber, periodDto.From.ColumnNumber};
        if (periodDto.To == null) return JsonValue.Create(from);

        var to = new[] {periodDto.To.LineNumber, periodDto.To.ColumnNumber};
        return JsonValue.Create(new[] {from, to});
    }

    private static JsonValue? SerializeTime(TimelineDto timelineDto)
    {
        return timelineDto.End == null
            ? JsonValue.Create(timelineDto.Start)
            : JsonValue.Create(new[] {timelineDto.Start, timelineDto.End});
    }

    private JsonObject SerializeRecord(RecordDto recordDto)
    {
        var response = new JsonObject();
        response.Add("t", SerializeTime(recordDto.Time));
        if (recordDto.Long != null)
            response.Add("l", recordDto.Long);
        var operations = recordDto.Operation.Select(SerializeOperation).ToArray();
        response.Add("o", JsonValue.Create(operations));

        return response;
    }

    private JsonObject SerializeOperation(OperationDto operationDto)
    {
        var response = new JsonObject();

        var type = SerializeType(operationDto.Type);
        if (type != null)
            response.Add("o", type);
        if (operationDto.Type == OperationTypeDto.Extra)
        {
            var extra = JsonSerializer.Deserialize<JsonValue>(operationDto.Extra);
            response.Add("activity", extra);
            return response;
        }

        var index = SerializePeriod(operationDto.Index);
        response.Add("i", index);

        if (operationDto.Value != null)
        {
            response.Add("a", operationDto.Value.Value.Length == 1
                             ? JsonValue.Create(operationDto.Value.Value[0])
                             : JsonValue.Create(operationDto.Value.Value));
        }

        if (operationDto.Remove != null)
        {
            var remove = operationDto.Remove.Select(x => new[] {x.Long, x.Count}).ToArray();
            response.Add("r", JsonValue.Create(remove));
        }

        if (operationDto.Select != null)
        {
            var select = operationDto.Select.Select(SerializeSelect).ToArray();
            response.Add("s", JsonValue.Create(select));
        }

        if (operationDto.Delete != null)
        {
            var delete = JsonSerializer.Deserialize<JsonValue>(operationDto.Delete);
            response.Add("d", delete);
        }

        return response;
    }

    private JsonValue? SerializeSelect(SelectDto selectDto)
    {
        var lineNumber = JsonValue.Create(selectDto.LineNumber);
        var tailMove = JsonValue.Create(selectDto.TailMove.Select(SerializeMove).ToArray());
        return JsonValue.Create(new[] {lineNumber, tailMove});
    }

    private JsonValue? SerializeMove(MoveDto moveDto)
    {
        var start = moveDto.Start;
        if (moveDto.End == null) return JsonValue.Create(start);

        var end = moveDto.End;
        return JsonValue.Create(new[] {start, end});
    }

    private string? SerializeType(OperationTypeDto type)
    {
        return type switch
        {
            OperationTypeDto.Compose => "c",
            OperationTypeDto.Delete => "d",
            OperationTypeDto.Input => "i",
            OperationTypeDto.MarkText => "k",
            OperationTypeDto.Select => "l",
            OperationTypeDto.Mouse => "m",
            OperationTypeDto.Rename => "n",
            OperationTypeDto.Move => "o",
            OperationTypeDto.Paste => "p",
            OperationTypeDto.Drag => "r",
            OperationTypeDto.SetValue => "s",
            OperationTypeDto.Cut => "x",
            OperationTypeDto.Extra => "e",
            OperationTypeDto.NoType => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}