using Microsoft.CodeAnalysis;

namespace CompilerService.Models
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
        
        public CompilationError(Esprima.ParserException parserException)
        {
            ErrorCode = "JsCompilationError";
            Message = parserException.Message;
            StartLine = parserException.LineNumber - 1;
            EndLine = parserException.LineNumber - 1;
            StartChar = parserException.Column;
            EndChar = parserException.Column;
        }
        
        public CompilationError(Jint.Runtime.JavaScriptException javaScriptException)
        {
            ErrorCode = "JsRuntimeError";
            Message = javaScriptException.Message;
            StartLine = javaScriptException.Location.Start.Line - 1;
            EndLine = javaScriptException.Location.End.Line - 1;
            StartChar = javaScriptException.Location.Start.Column;
            EndChar = javaScriptException.Location.End.Column;
        }
    }
}
