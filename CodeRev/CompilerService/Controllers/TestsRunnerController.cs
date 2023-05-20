using System;
using CompilerService.Models;
using CompilerService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CompilerService.Controllers;

[Route("api/tests")]
[EnableCors]
[ApiController]
public class TestsRunnerController : ControllerBase
{
    private readonly AssemblyTestingService assemblyTestingService;

    public TestsRunnerController(AssemblyTestingService assemblyTestingService)
    {
        this.assemblyTestingService = assemblyTestingService;
    }

    [HttpPost("run")]
    public ActionResult<TestsRunResult> RunTests([FromBody]TestsRunRequest req)
    {
        // todo брать код теста из БД по id задачи
        var res = assemblyTestingService.RunTests(req.Code, @"using NUnit.Framework;

namespace CodeRevSolution;

[TestFixture]
public class SomeTestCode
{
    [Test]
    public void Should_return_129()
    {
        var instance = new SomeProg();
        var result = instance.MethodToCheck();
        
        Assert.AreEqual(result, 129);
    }
    
        [Test]
        public void Should_return_128()
        {
            var instance = new SomeProg();
            var result = instance.MethodToCheck();
            
            Assert.AreEqual(result, 128);
        }
}");
        
        return Ok(res);
    }
}