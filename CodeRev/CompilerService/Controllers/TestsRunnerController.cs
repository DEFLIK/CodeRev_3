using System;
using CompilerService.Models;
using CompilerService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskTestsProvider;

namespace CompilerService.Controllers;

[Route("api/tests")]
[EnableCors]
[ApiController]
public class TestsRunnerController : ControllerBase
{
    private readonly AssemblyTestingService assemblyTestingService;
    private readonly ITaskTestsProviderClient taskTestsProviderClient;

    public TestsRunnerController(AssemblyTestingService assemblyTestingService, ITaskTestsProviderClient taskTestsProviderClient)
    {
        this.assemblyTestingService = assemblyTestingService;
        this.taskTestsProviderClient = taskTestsProviderClient;
    }

    [HttpPost("run")]
    public ActionResult<TestsRunResult> RunTests([FromBody]TestsRunRequest req)
    {
        var res = assemblyTestingService.RunTests(req.Code, taskTestsProviderClient.GetTaskTestsCodeBySolutionId(req.TaskSolutionId));
        
        return Ok(res);
    }
}