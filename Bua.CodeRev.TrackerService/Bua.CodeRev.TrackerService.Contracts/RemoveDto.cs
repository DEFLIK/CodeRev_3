using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts;

[DataContract]
public class RemoveDto
{
    [DataMember]
    public int Long { get; set; }
    
    [DataMember]
    public int Count { get; set; }
}