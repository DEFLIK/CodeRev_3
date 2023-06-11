using System.Collections.Generic;

namespace CompilerService.Models;

public class TestsRunResult
{
    public List<string> PassedTestCases { get; set; } = new();
    public Dictionary<string, string> FailedTestCases { get; set; } = new();
    public bool IsCompiledSuccessfully { get; set; }

}