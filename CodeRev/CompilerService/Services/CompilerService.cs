using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CompilerService.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace CompilerService.Services;

public interface ICompilerService
{
    /// <summary>
    /// Ассинхронно производит выполнение кода
    /// </summary>
    /// <param name="code">Код в виде Plain-Text</param>
    /// <returns>Результат выполнения (подобно консольному выводу/ошибке)</returns>
    /// <exception cref="ArgumentException">Неверное указана входная точка</exception>
    public ExecutionResult Execute(string code, EntryPoint entryPoint);
}

public class CompilerService : ICompilerService
{
    private static readonly string[] _dependencies =
    {
        "System.Private.CoreLib.dll",
        "System.Console.dll",
        "System.Runtime.dll"
    };

    private readonly string assemblyPath;

    public CompilerService()
    {
        assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)
                        ?? throw new NullReferenceException(
                            "Failed to create CompileService instance: unable to get assembly location." +
                            "Make sure to save your assembly if you loading it from bytes array");
    }

    private static IEnumerable<Diagnostic> GetErrorDiagnostics(EmitResult emitResult) =>
        emitResult
            .Diagnostics
            .Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

    /// <returns>Консольный вывод запущенной сборки</returns>
    /// <exception cref="ArgumentException">В сборке отсутствует требуемая входная точка</exception>
    private static IEnumerable<string> RunAssemblyFromStream(MemoryStream ms, EntryPoint entryPoint)
    {
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var entryClassInstance = assembly
            .CreateInstance($"{entryPoint.NamespaceName}.{entryPoint.ClassName}") 
            ?? throw new ArgumentException(
                $"Unable to create instance of '{entryPoint.ClassName}' at {entryPoint.NamespaceName} from input solution");

        var methodInfo = entryClassInstance
            .GetType()
            .GetMethod(entryPoint.MethodName)
            ?? throw new ArgumentException(
                $"Unable to invoke '{entryPoint.MethodName}' at {entryPoint.ClassName} from input solution");

        using var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            methodInfo.Invoke(null, null);
        }
        catch (TargetInvocationException exception)
        {
            var inner = exception.InnerException;

            return inner is null 
                ? new[] { $"Серверная ошибка выполнения: { exception }" } 
                : new[] { inner.ToString() };
        }

        return sw.ToString().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
    }

    public ExecutionResult Execute(string code, EntryPoint entryPoint)
    {
        var compilation = GetCompilation(code);

        using var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);

        var errors = new List<CompilationError>();
        IEnumerable<string> output = new List<string>();

        if (!emitResult.Success)
            errors.AddRange(GetErrorDiagnostics(emitResult).Select(diagnostic => new CompilationError(diagnostic)));
        else
            output = RunAssemblyFromStream(ms, entryPoint);

        return new ExecutionResult()
        {
            Success = emitResult.Success,
            Output = output,
            Errors = errors
        };
    }

    private CSharpCompilation GetCompilation(string code) =>
        CSharpCompilation.Create(
            Path.GetRandomFileName(),
            new[] { CSharpSyntaxTree.ParseText(code) },
            GetMetadataReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private MetadataReference[] GetMetadataReferences() =>
        _dependencies.Select(refString =>
                MetadataReference
                    .CreateFromFile(Path.Combine(assemblyPath, refString)))
            .ToArray<MetadataReference>();
}