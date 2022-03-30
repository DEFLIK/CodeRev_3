using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.CompilerService.Core.Models;
using Bua.CodeRev.CompilerService.Core.Services.CompileService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bua.CodeRev.CompilerService.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompileController : ControllerBase
    {
        private readonly ICompileService _compiler;

        public CompileController(ICompileService _compiler)
        {
            this._compiler = _compiler;
        }

        [HttpGet]
        public ExecutionResult CompileAndExecute(string code)
        {
            var res = _compiler.CompileAndExecute(code);
            GC.Collect();
            return res;
        }
    }
}
