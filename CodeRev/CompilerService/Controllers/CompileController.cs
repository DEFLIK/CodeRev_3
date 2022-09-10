using System;
using CodeRev.CompilerService.Helpers;
using CodeRev.CompilerService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CodeRev.CompilerService.Controllers
{
    [Route("api/compile")]
    [EnableCors]
    [ApiController]
    public class CompileController : ControllerBase
    {
        private readonly ICompiler compiler;

        public CompileController(ICompiler compiler)
        {
            this.compiler = compiler;
        }

        [HttpPut("execute")]
        public ActionResult<ExecutionResult> Execute([FromBody]ExecutionRequest req)
        {
            ExecutionResult res;
            try
            {
                res = compiler.Execute(req.Code, req.EntryPoint);
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
