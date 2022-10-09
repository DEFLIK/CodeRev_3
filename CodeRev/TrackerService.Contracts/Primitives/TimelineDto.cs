using System.Runtime.Serialization;

namespace TrackerService.Contracts.Primitives;

[DataContract]
public class TimelineDto
{
    [DataMember]
    public int Start { get; set; }

    [DataMember]
    public int? End { get; set; }
}