using System.Runtime.Serialization;

namespace TrackerService.Contracts.Primitives;

[DataContract]
public class PeriodDto
{
    [DataMember]
    public IndexDto From { get; set; }

    [DataMember]
    public IndexDto? To { get; set; }
}