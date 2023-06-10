using System;
using CompilerService.Models;
using CompilerService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Enums;

namespace CompilerService.Controllers
{
    [Route("api/compile")]
    [EnableCors]
    [ApiController]
    public class CompileController : ControllerBase
    {
        private readonly CSharpCompilerService cSharpCompilerService;
        private readonly JsCompilerService jsCompilerService;

        public CompileController(CSharpCompilerService cSharpCompilerService, JsCompilerService jsCompilerService)
        {
            this.cSharpCompilerService = cSharpCompilerService;
            this.jsCompilerService = jsCompilerService;
        }

        [HttpPut("execute")]
        public ActionResult<ExecutionResult> Execute([FromBody]ExecutionRequest req)
        {
            ExecutionResult res;
            try
            {
                res = req.ProgrammingLanguage switch
                {
                    ProgrammingLanguage.CSharp => cSharpCompilerService.Execute(req.Code, req.EntryPoint),
                    ProgrammingLanguage.JavaScript => jsCompilerService.Execute(req.Code, req.EntryPoint),
                    _ => throw new ArgumentOutOfRangeException($"Unsupported programming language: {req.ProgrammingLanguage}")
                };
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
