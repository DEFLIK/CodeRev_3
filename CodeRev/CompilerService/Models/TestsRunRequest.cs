using System;

namespace CompilerService.Models;

public class TestsRunRequest
{
    public string Code { get; set; }
    public Guid TaskId { get; set; }
}