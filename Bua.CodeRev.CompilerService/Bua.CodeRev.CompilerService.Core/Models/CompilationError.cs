using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Bua.CodeRev.CompilerService.Core.Models
{
    public class CompilationError
    {
        public string ErrorCode { get; }
        public string Message { get; }
        public int StartChar { get; }
        public int EndChar { get; }
        public int StartLine { get; }
        public int EndLine { get; }

        public CompilationError(Diagnostic diagnostic)
        {
            var lineSpan = diagnostic.Location.GetLineSpan();

            ErrorCode = diagnostic.Id;
            Message = diagnostic.GetMessage();
            StartLine = lineSpan.StartLinePosition.Line;
            EndLine = lineSpan.EndLinePosition.Line;
            StartChar = lineSpan.StartLinePosition.Character;
            EndChar = lineSpan.EndLinePosition.Character;
        }
    }
}
