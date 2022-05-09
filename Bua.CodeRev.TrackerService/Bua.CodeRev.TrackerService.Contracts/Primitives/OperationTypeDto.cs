using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts.Primitives;

[DataContract]
public enum OperationTypeDto
{
    /// <summary>
    ///     Compose
    /// </summary>
    [EnumMember]
    c = 0,
    /// <summary>
    ///     Delete
    /// </summary>
    [EnumMember]
    d = 1,
    /// <summary>
    ///     Input
    /// </summary>
    [EnumMember]
    i = 2,
    /// <summary>
    ///     MarkText
    /// </summary>
    [EnumMember]
    k = 3,
    /// <summary>
    ///     Select
    /// </summary>
    [EnumMember]
    l = 4,
    /// <summary>
    ///     Mouse
    /// </summary>
    [EnumMember]
    m = 5,
    /// <summary>
    ///     Rename
    /// </summary>
    [EnumMember]
    n = 6,
    /// <summary>
    ///     Move
    /// </summary>
    [EnumMember]
    o = 7,
    /// <summary>
    ///     Paste
    /// </summary>
    [EnumMember]
    p = 8,
    /// <summary>
    ///     Drag
    /// </summary>
    [EnumMember]
    r = 9,
    /// <summary>
    ///     SetValue
    /// </summary>
    [EnumMember]
    s = 10,
    /// <summary>
    ///     Cut
    /// </summary>
    [EnumMember]
    x = 11,
    /// <summary>
    ///     Extra
    /// </summary>
    [EnumMember]
    e = 12,
    [EnumMember]
    NoType
}