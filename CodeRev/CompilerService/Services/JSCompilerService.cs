using System;
using System.Collections.Generic;
using CompilerService.Models;
using Jint;

namespace CompilerService.Services;

public class JsCompilerService: ICompilerService
{

    public ExecutionResult Execute(string code, EntryPoint entryPoint)
    {
        var output = new List<string>();
        var engine = new Engine()
           .SetValue("log", new Action<object>(obj => output.Add(obj.ToString() ?? "null")));

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
            Output = output
        };
    }
}