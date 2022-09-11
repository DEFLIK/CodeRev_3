using System.Runtime.Serialization;

namespace TrackerService.Contracts;

[DataContract]
public class LastCodeDto
{
    [DataMember] public string? Code { get; set; }
}