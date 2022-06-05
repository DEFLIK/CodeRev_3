using System.Text.Json;
using Bua.CodeRev.TrackerService.Contracts.Actions;
using Bua.CodeRev.TrackerService.Contracts.Primitives;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Newtonsoft.Json.Linq;

namespace Bua.CodeRev.TrackerService.DomainCore.Deserialize;

public class Deserializer : IDeserializer
{
    public TaskRecordDto ParseRequestDto(JsonElement request)
    {
        var jObject = JObject.Parse(request.ToString());
        var taskSolutionId = (Guid) jObject["TaskSolutionId"];
        var records = jObject["Records"].Select(ParseRecord).ToArray();
        var saveTime = (decimal) jObject["SaveTime"];
        var recordChunk = new RecordChunkDto {SaveTime = saveTime, Records = records, Code = (string)jObject["Code"]};
        var code = (string) jObject["Code"];
        return new TaskRecordDto
        {
            TaskSolutionId = taskSolutionId,
            RecordChunks = new[] {recordChunk},
            Code = code
        };
    }

    private RecordDto ParseRecord(JToken record)
    {
        var time = ParseTimeline(record["t"]);
        var count = (int?) record["l"];
        var t = record["o"];
        var operation = record["o"]?.Select(ParseOperation).ToArray();
        return new RecordDto {Time = time, Long = count, Operation = operation};
    }

    private TimelineDto ParseTimeline(JToken? timeline)
    {
        if (timeline?.Type == JTokenType.Array)
        {
            var start = (int) timeline.FirstOrDefault();
            var end = (int) timeline.LastOrDefault();
            return new TimelineDto {Start = start, End = end};
        }

        return new TimelineDto {Start = (int) timeline};
    }

    private OperationDto ParseOperation(JToken? operation)
    {
        var type = ParseOperationType(operation["o"]);
        var period = ParsePeriod(operation["i"]);
        var value = ParseValue(operation["a"]);
        var remove = operation["r"]?.Select(ParseRemove).ToArray();
        var select = operation["s"]?.Select(ParseSelect).ToArray();

        return new OperationDto
        {
            Type = type,
            Index = period,
            Value = value,
            Remove = remove,
            Select = select
        };
    }

    private OperationTypeDto ParseOperationType(JToken? type)
    {
        return (string) type! switch
        {
            "c" => OperationTypeDto.Compose,
            "d" => OperationTypeDto.Delete,
            "i" => OperationTypeDto.Input,
            "k" => OperationTypeDto.MarkText,
            "l" => OperationTypeDto.Select,
            "m" => OperationTypeDto.Mouse,
            "n" => OperationTypeDto.Rename,
            "o" => OperationTypeDto.Move,
            "p" => OperationTypeDto.Paste,
            "r" => OperationTypeDto.Drag,
            "s" => OperationTypeDto.SetValue,
            "x" => OperationTypeDto.Cut,
            "e" => OperationTypeDto.Extra,
            null => OperationTypeDto.NoType,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private SelectDto ParseSelect(JToken? select)
    {
        var lineNumber = (int) select[0];
        var move = select[1].Select(ParseMove).ToArray();

        return new SelectDto {LineNumber = lineNumber, TailMove = move};
    }

    private MoveDto ParseMove(JToken? move)
    {
        if (move.Type == JTokenType.Array)
        {
            var start = (int) move[0];
            var end = (int) move[1];
            return new MoveDto {Start = start, End = end};
        }

        return new MoveDto {Start = (int) move};
    }

    private PeriodDto ParsePeriod(JToken? period)
    {
        if (period[0].Type == JTokenType.Array)
            return new PeriodDto
            {
                From = new IndexDto {LineNumber = (int) period[0][0], ColumnNumber = (int) period[0][1]},
                To = new IndexDto {LineNumber = (int) period[1][0], ColumnNumber = (int) period[1][1]}
            };
        return new PeriodDto
        {
            From = new IndexDto
            {
                LineNumber = (int) period[0],
                ColumnNumber = (int) period[1]
            }
        };
    }

    private RemoveDto ParseRemove(JToken? remove)
    {
        return new RemoveDto {Count = (int) remove[0], Long = (int) remove[1]};
    }

    private ValueDto ParseValue(JToken? value)
    {
        if (value == null)
            return null;
        if (value.Type == JTokenType.Array) return new ValueDto {Value = value.Values<string>().ToArray()};

        return new ValueDto {Value = new[] {(string) value}};
    }
}