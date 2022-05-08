using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts.Primitives;

[DataContract]
public class IndexDto
{
    [DataMember]
    public int LineNumber { get; set; }
    
    [DataMember]
    public int ColumnNumber { get; set; }
}