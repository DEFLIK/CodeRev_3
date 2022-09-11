using System.Runtime.Serialization;

namespace TrackerService.Contracts.Actions;

[DataContract]
public class SelectDto
{
    [DataMember] public int LineNumber { get; set; }

    [DataMember] public MoveDto[] TailMove { get; set; }
}