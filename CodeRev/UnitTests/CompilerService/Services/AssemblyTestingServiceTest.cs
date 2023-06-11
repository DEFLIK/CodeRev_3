using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests.CompilerService.Services;

public class AssemblyTestingServiceTest
{

    private global::CompilerService.Services.AssemblyTestingService assemblyTestingService;

    [SetUp]
    public void SetUp()
    {
        assemblyTestingService = new global::CompilerService.Services.AssemblyTestingService();
        
    }

    [TestCaseSource(nameof(GetCases))]
    public void Tests_should_run_correctly((string UserCode, string TestCode, bool ExpectedResult, string ExpectedResultText) testCase)
    {
        var result = assemblyTestingService.RunTests(testCase.UserCode, testCase.TestCode);

        result.IsCompiledSuccessfully.Should().BeTrue();
        result.PassedTestCases.Should().BeEquivalentTo(new List<string> {"Should_return_129"});
        result.FailedTestCases.Should().BeEquivalentTo(new Dictionary<string, string> { { "Should_return_128", "Expected: 129  But was:  128" } });
    }

    private static IEnumerable<(string UserCode, string TestCode, bool ExpectedResult, string ExpectedResultText)> GetCases()
    {
        yield return (File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharpCompilerService/TestData/SomeUserCode.cs.txt")), 
            File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharpCompilerService/TestData/SomeTests.cs.txt")), 
            true, "");
    }
}