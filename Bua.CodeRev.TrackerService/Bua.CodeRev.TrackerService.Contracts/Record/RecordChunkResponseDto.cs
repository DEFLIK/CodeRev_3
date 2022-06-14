﻿using System.Runtime.Serialization;
using System.Text.Json.Nodes;

namespace Bua.CodeRev.TrackerService.Contracts.Record;

[DataContract]
public class RecordChunkResponseDto
{
    [DataMember] public decimal SaveTime { get; set; }
    [DataMember] public string Code { get; set; }
    [DataMember] public JsonObject[] Records { get; set; }
}