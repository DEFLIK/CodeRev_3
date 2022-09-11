using System.Collections.Generic;

namespace CompilerService.Models
{
    public class ExecutionResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Output { get; set; }
        public IEnumerable<CompilationError> Errors { get; set; }
    }
}
