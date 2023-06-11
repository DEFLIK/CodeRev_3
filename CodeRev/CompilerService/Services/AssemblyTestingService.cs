using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using CompilerService.Models;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace CompilerService.Services;

public class AssemblyTestingService
{
    private static readonly string[] DotNetDependencies =
    {
        "System.Private.CoreLib.dll",
        "System.Console.dll",
        "System.Runtime.dll",
        "netstandard.dll"
    };
    private static readonly string[] AdditionalDependencies =
    {
        "nunit.framework.dll"
    };

    private readonly string assemblyPath;
    private readonly string workingPath;

    public AssemblyTestingService()
    {
        assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)
                       ?? throw new NullReferenceException(
                           "Failed to create AssemblyTestingService instance: unable to get assembly location." +
                           "Make sure to save your assembly if you loading it from bytes array");
        workingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
    }
    
    public TestsRunResult RunTests(string code, string testsCode)
    {
        var dotNetRefs = CSharpCompilerService.GetMetadataReferences(assemblyPath, DotNetDependencies).ToArray();
        var additionalRefs = CSharpCompilerService.GetMetadataReferences(workingPath, AdditionalDependencies).ToArray();
        var testCodeMetadataReferences = dotNetRefs.Concat(additionalRefs);
        var testCodeCompilation = CSharpCompilerService.GetCompilation(new [] {code, testsCode}, testCodeMetadataReferences);
        
        using var ms = new MemoryStream();
        var result = testCodeCompilation.Emit(ms);

        if (!result.Success)
            return new TestsRunResult { IsCompiledSuccessfully = false };
        
        var context = new AssemblyLoadContext(testCodeCompilation.AssemblyName, isCollectible: true);
        var assembly = context.LoadFromStream(new MemoryStream(ms.ToArray()));
        
        var testRunner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
        testRunner.Load(assembly, new Dictionary<string, object>() {{"WorkDirectory", workingPath}});
        var testResult = testRunner.Run(new DeafTestListener(), NUnit.Framework.Internal.TestFilter.Empty);

        var testCases = testResult.Children.First().Children.First().Children.ToArray();
        var passedTestCases = testCases.Where(testCase => testCase.ResultState == ResultState.Success)
                                       .Select(testCase => testCase.Name)
                                       .ToList();
        var failedTestCases = testCases.Where(testCase => testCase.ResultState != ResultState.Success)
                                       .ToDictionary(
                                            testCase => testCase.Name, 
                                            testCase => testCase.Message?.Trim().Replace("\r", "").Replace("\n", "") 
                                                        ?? "No error message");

        context.Unload();
        return new TestsRunResult
        {
            IsCompiledSuccessfully = true, 
            PassedTestCases = passedTestCases,
            FailedTestCases = failedTestCases
        };
    }
}

public class DeafTestListener : ITestListener
{
    public void TestStarted(ITest test)
    {
    }

    public void TestFinished(ITestResult result)
    {
    }

    public void TestOutput(TestOutput output)
    {
    }

    public void SendMessage(TestMessage message)
    {
    }
}
