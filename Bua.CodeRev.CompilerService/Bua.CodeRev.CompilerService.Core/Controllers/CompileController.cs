using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bua.CodeRev.CompilerService.Core.Models;
using Bua.CodeRev.CompilerService.Core.Services.CompileService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bua.CodeRev.CompilerService.Core.Controllers
{
    [Route("api/compile")]
    [EnableCors]
    [ApiController]
    public class CompileController : ControllerBase
    {
        private readonly ICompileService _compiler;

        public CompileController(ICompileService _compiler)
        {
            this._compiler = _compiler;
        }

        [HttpPut("execute")]
        public ActionResult<ExecutionResult> Execute([FromBody]ExecutionRequest req)
        {
            ExecutionResult res;
            try
            {
                res = _compiler.Execute(req.Code, req.EntryPoint);
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
