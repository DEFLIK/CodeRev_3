using Bua.CodeRev.TrackerService.Contracts;
using Bua.CodeRev.TrackerService.Contracts.Primitives;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Newtonsoft.Json.Linq;

namespace Bua.CodeRev.TrackerService.DomainCore;

public class Serializer : ISerializer
{
    public JArray? Serialize(RecordChunkDto[]? recordChunks)
    {
        if (recordChunks == null)
            return null;
        var result = new List<JObject>();
        foreach (var recordChunk in recordChunks) result.AddRange(recordChunk.Records.Select(SerializeRecord));

        return new JArray(result);
    }

    private JObject SerializeRecord(RecordDto recordDto)
    {
        var response = new JObject();
        response.Add("t", SerializeTime(recordDto.Time));
        if (recordDto.Long != null)
            response.Add("l", recordDto.Long);
        var operations = recordDto.Operation.Select(SerializeOperation).ToArray();
        response.Add("o", JToken.FromObject(operations));

        return response;
    }

    private JObject SerializeOperation(OperationDto operationDto)
    {
        var response = new JObject();

        var type = SerializeType(operationDto.Type);
        if (type != null)
            response.Add("o", type);

        var index = SerializePeriod(operationDto.Index);
        response.Add("i", index);

        if (operationDto.Value != null) response.Add("a", JToken.FromObject(operationDto.Value.Value));

        if (operationDto.Remove != null)
        {
            var remove = operationDto.Remove.Select(x => JToken.FromObject(new[] {x.Long, x.Count})).ToArray();
            response.Add("r", JToken.FromObject(remove));
        }

        if (operationDto.Select != null)
        {
            var select = operationDto.Select.Select(SerializeSelect).ToArray();
            response.Add("s", JToken.FromObject(select));
        }

        return response;
    }

    private JToken SerializeSelect(SelectDto selectDto)
    {
        var lineNumber = JToken.FromObject(selectDto.LineNumber);
        var tailMove = JToken.FromObject(selectDto.TailMove.Select(SerializeMove).ToArray());
        return JToken.FromObject(new[] {lineNumber, tailMove});
    }

    private JToken SerializeMove(MoveDto moveDto)
    {
        var start = JToken.FromObject(moveDto.Start);
        if (moveDto.End != null)
        {
            var end = JToken.FromObject(moveDto.End);
            return JToken.FromObject(new[] {start, end});
        }

        return JToken.FromObject(start);
    }

    private JToken SerializePeriod(PeriodDto periodDto)
    {
        var from = JToken.FromObject(new[] {periodDto.From.LineNumber, periodDto.From.ColumnNumber});
        if (periodDto.To != null)
        {
            var to = JToken.FromObject(new[] {periodDto.To.LineNumber, periodDto.To.ColumnNumber});
            return JToken.FromObject(new[] {from, to});
        }

        return JToken.FromObject(from);
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

    private JToken SerializeTime(TimelineDto timelineDto)
    {
        return timelineDto.End == null
            ? new JValue(timelineDto.Start)
            : JToken.FromObject(new[] {timelineDto.Start, timelineDto.End});
    }
}