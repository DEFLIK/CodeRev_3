using System.Runtime.Serialization;
using System.Text.Json.Nodes;

namespace TrackerService.Contracts.Record;

[DataContract]
public class TaskRecordRequestDto
{
    [DataMember] public Guid TaskSolutionId { get; set; }
    [DataMember] public string Code { get; set; }
    [DataMember] public decimal SaveTime { get; set; }
    [DataMember] public JsonValue[] Records { get; set; }
}