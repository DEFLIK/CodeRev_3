using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts;

[DataContract]
public class LastCodeDto
{
    [DataMember] public string? Code { get; set; }
}