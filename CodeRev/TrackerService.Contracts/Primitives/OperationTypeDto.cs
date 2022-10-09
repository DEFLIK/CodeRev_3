using System.Runtime.Serialization;

namespace TrackerService.Contracts.Primitives;

[DataContract]
public enum OperationTypeDto
{
    /// <summary>
    ///     c
    /// </summary>
    [EnumMember]
    Compose = 0,

    /// <summary>
    ///     d
    /// </summary>
    [EnumMember] Delete = 1,

    /// <summary>
    ///     i
    /// </summary>
    [EnumMember] Input = 2,

    /// <summary>
    ///     k
    /// </summary>
    [EnumMember] MarkText = 3,

    /// <summary>
    ///     l
    /// </summary>
    [EnumMember] Select = 4,

    /// <summary>
    ///     m
    /// </summary>
    [EnumMember] Mouse = 5,

    /// <summary>
    ///     n
    /// </summary>
    [EnumMember] Rename = 6,

    /// <summary>
    ///     o
    /// </summary>
    [EnumMember] Move = 7,

    /// <summary>
    ///     p
    /// </summary>
    [EnumMember] Paste = 8,

    /// <summary>
    ///     r
    /// </summary>
    [EnumMember] Drag = 9,

    /// <summary>
    ///     s
    /// </summary>
    [EnumMember] SetValue = 10,

    /// <summary>
    ///     x
    /// </summary>
    [EnumMember] Cut = 11,

    /// <summary>
    ///     e
    /// </summary>
    [EnumMember] Extra = 12,
    [EnumMember] NoType
}