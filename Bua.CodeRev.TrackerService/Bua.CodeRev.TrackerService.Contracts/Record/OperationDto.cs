using System.Runtime.Serialization;
using Bua.CodeRev.TrackerService.Contracts.Actions;
using Bua.CodeRev.TrackerService.Contracts.Primitives;

namespace Bua.CodeRev.TrackerService.Contracts.Record;

[DataContract]
public class OperationDto
{
    /// <summary>
    ///     o
    /// </summary>
    [DataMember]
    public OperationTypeDto Type { get; set; }

    /// <summary>
    ///     i
    /// </summary>
    [DataMember]
    public PeriodDto Index { get; set; }

    /// <summary>
    ///     a
    /// </summary>
    [DataMember]
    public ValueDto? Value { get; set; }

    /// <summary>
    ///     r
    /// </summary>
    [DataMember]
    public RemoveDto[]? Remove { get; set; }

    /// <summary>
    ///     s
    /// </summary>
    [DataMember]
    public SelectDto[]? Select { get; set; }

    /// <summary>
    ///     e
    /// </summary>
    [DataMember]
    public string? Extra { get; set; }

    /// <summary>
    ///     d
    /// </summary>
    [DataMember]
    public string? Delete { get; set; }
}