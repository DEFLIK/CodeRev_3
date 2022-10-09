using System.Runtime.Serialization;

namespace TrackerService.Contracts.Primitives;

[DataContract]
public class ValueDto
{
    [DataMember]
    public string[] Value { get; set; }
}