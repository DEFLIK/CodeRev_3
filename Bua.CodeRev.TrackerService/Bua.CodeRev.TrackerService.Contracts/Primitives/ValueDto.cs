﻿using System.Runtime.Serialization;

namespace Bua.CodeRev.TrackerService.Contracts.Primitives;

[DataContract]
public class ValueDto
{
    [DataMember]
    public string[] Value { get; set; }
}