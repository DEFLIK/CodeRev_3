using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Bua.CodeRev.TrackerService.Contracts.Record;

[DataContract]
public class RecordsRequestDto
{
    [DataMember]
    public ObjectId Id { get; set; }

    [DataMember]
    public Guid TaskSolutionId { get; set; }
    
    [DataMember]
    public RecordDto[] Records { get; set; }
}