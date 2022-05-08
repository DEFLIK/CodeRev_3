﻿using System.Runtime.Serialization;
using Bua.CodeRev.TrackerService.Contracts.Primitives;

namespace Bua.CodeRev.TrackerService.Contracts.Record;
[DataContract]
public class RecordDto
{
    [DataMember]
    public TimelineDto Time { get; set; }
    
    [DataMember]
    public int? Long { get; set; }
    
    [DataMember]
    public OperationDto[] Operation { get; set; }
}