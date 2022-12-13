using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using TrackerService.Contracts.Actions;
using TrackerService.Contracts.Primitives;
using TrackerService.Contracts.Record;

namespace TrackerService.Infrastructure.Deserialize;

public class Deserializer : IDeserializer
{
    public TaskRecordDto ParseRequestDto(TaskRecordRequestDto request)
    {
        var records = request.Records.Select(ParseRecord).ToArray();
        var recordChunk = new RecordChunkDto {SaveTime = request.SaveTime, Records = records, Code = request.Code};
        return new TaskRecordDto
        {
            TaskSolutionId = request.TaskSolutionId,
            RecordChunks = new[] {recordChunk},
            Code = request.Code
        };
    }

    private RecordDto ParseRecord(JsonValue record)
    {
        var jObject = JObject.Parse(record.ToString());
        var time = ParseTimeline(jObject["t"]);
        var count = (int?) jObject["l"];
        var t = jObject["o"];
        var operation = jObject["o"]?.Select(ParseOperation).ToArray();
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
        if (type == OperationTypeDto.Extra)
        {
            var extra = operation["activity"].ToString();
            return new OperationDto
            {
                Type = type,
                Extra = extra
            };
        }

        var period = ParsePeriod(operation["i"]);
        var value = ParseValue(operation["a"]);
        var remove = operation["r"]?.Select(ParseRemove).ToArray();
        var select = operation["s"]?.Select(ParseSelect).ToArray();
        var delete = operation["d"]?.ToString();

        return new OperationDto
        {
            Type = type,
            Index = period,
            Value = value,
            Remove = remove,
            Select = select,
            Delete = delete
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