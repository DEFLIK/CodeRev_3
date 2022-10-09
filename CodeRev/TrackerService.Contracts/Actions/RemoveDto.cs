using System.Runtime.Serialization;

namespace TrackerService.Contracts.Actions;

[DataContract]
public class RemoveDto
{
    [DataMember] public int Long { get; set; }

    [DataMember] public int Count { get; set; }
}