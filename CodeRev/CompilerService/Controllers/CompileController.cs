using System;
using CompilerService.Models;
using CompilerService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CompilerService.Controllers
{
    [Route("api/compile")]
    [EnableCors]
    [ApiController]
    public class CompileController : ControllerBase
    {
        private readonly ICompilerService compilerService;

        public CompileController(ICompilerService compilerService)
        {
            this.compilerService = compilerService;
        }

        [HttpPut("execute")]
        public ActionResult<ExecutionResult> Execute([FromBody]ExecutionRequest req)
        {
            ExecutionResult res;
            try
            {
                res = compilerService.Execute(req.Code, req.EntryPoint);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid entry point");
            }
            
            // Следует использовать при включенной серверной сборке мусора,
            // во избежание быстрого накопления памяти после сборки решения
            //GC.Collect();

            return Ok(res);
        }
    }
}
