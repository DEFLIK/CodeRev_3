using System.Runtime.Serialization;

namespace TrackerService.Contracts.Primitives;

[DataContract]
public class IndexDto
{
    [DataMember]
    public int LineNumber { get; set; }

    [DataMember]
    public int ColumnNumber { get; set; }
}