using System.Runtime.Serialization;
using Bua.CodeRev.TrackerService.Contracts.Primitives;

namespace Bua.CodeRev.TrackerService.Contracts;

[DataContract]
public class SelectDto
{
    [DataMember]
    public int LineNumber { get; set; }
    
    [DataMember]
    public MoveDto[] TailMove { get; set; }
}