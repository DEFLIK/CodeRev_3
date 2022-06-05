using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts.Record;

[DataContract]
public class RecordChunkDto
{
    [DataMember] public decimal SaveTime { get; set; }

    [DataMember] public string Code { get; set; }
    [DataMember] public RecordDto[] Records { get; set; }
}