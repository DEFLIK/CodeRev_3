using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bua.CodeRev.CompilerService.Core.Models
{
    public class ExecutionResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Output { get; set; }
        public IEnumerable<CompilationError> Errors { get; set; }
    }
}
