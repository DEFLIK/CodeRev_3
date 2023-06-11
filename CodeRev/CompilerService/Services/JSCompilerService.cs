using System;
using CompilerService.Models;
using Jint;

namespace CompilerService.Services;

public class JsCompilerService: ICompilerService
{

    public ExecutionResult Execute(string code, EntryPoint entryPoint)
    {
        var output = "";
        var engine = new Engine()
           .SetValue("log", new Action<object>(obj => output = obj.ToString()));

        try
        {
            engine.Execute(code);
        }
        catch (Esprima.ParserException parserException)
        {
            return new ExecutionResult
            {
                Success = false,
                Errors = new[] { new CompilationError(parserException) }
            };
        }
        catch (Jint.Runtime.JavaScriptException javaScriptException)
        {
            return new ExecutionResult
            {
                Success = false,
                Errors = new[] { new CompilationError(javaScriptException) }
            };
        }

        return new ExecutionResult
        {
            Success = true,
            Output = new[] { output }
        };
    }
}