using System.Runtime.Serialization;

namespace TrackerService.Contracts.Actions;

[DataContract]
public class MoveDto
{
    [DataMember] public int Start { get; set; }

    [DataMember] public int? End { get; set; }
}