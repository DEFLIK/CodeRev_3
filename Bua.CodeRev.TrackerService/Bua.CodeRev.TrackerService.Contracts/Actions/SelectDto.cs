using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts.Actions;

[DataContract]
public class SelectDto
{
    [DataMember] public int LineNumber { get; set; }

    [DataMember] public MoveDto[] TailMove { get; set; }
}