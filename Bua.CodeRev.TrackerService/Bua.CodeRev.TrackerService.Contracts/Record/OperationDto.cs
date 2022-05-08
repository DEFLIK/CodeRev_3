using System.Runtime.Serialization;
using Bua.CodeRev.TrackerService.Contracts.Primitives;

namespace Bua.CodeRev.TrackerService.Contracts.Record;
[DataContract]
public class OperationDto
{
    [DataMember]
    public OperationTypeDto Type { get; set; }// o
    
    [DataMember]
    public PeriodDto Index { get; set; }// i
    
    [DataMember]
    public ValueDto? Value { get; set; }// a
    
    [DataMember]
    public RemoveDto[]? Remove { get; set; }// r
    
    [DataMember]
    public SelectDto[]? Select { get; set; }// s
}