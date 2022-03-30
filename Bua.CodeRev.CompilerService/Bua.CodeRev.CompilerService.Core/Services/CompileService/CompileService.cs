using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bua.CodeRev.CompilerService.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Bua.CodeRev.CompilerService.Core.Services.CompileService
{
    public class CompileService : ICompileService
    {
        private const string InputProgramEntryNamespace = "CodeRevSolution";
        private const string InputProgramEntryClass = "Program";
        private const string InputProgramEntryMethod = "Main";

        private static readonly string[] Dependencies = new string[]
        {
            "System.Private.CoreLib.dll",
            "System.Console.dll",
            "System.Runtime.dll"
        };

        private readonly string _assemblyPath;

        public CompileService()
        {
            _assemblyPath = Path
                .GetDirectoryName(typeof(object).Assembly.Location) 
                ?? throw new NullReferenceException("Failed to create CompileService instance: unable to get assembly location");
        }

        private static IEnumerable<Diagnostic> GetDiagnostics(EmitResult emitResult) =>
            emitResult
                .Diagnostics
                .Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

        /// <returns>Консольный вывод запущенной сборки</returns>
        /// <exception cref="ArgumentException">В сборке отсутствует требуемая входная точка</exception>
        private static IEnumerable<string> RunAssemblyFromStream(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var entryClassInstance = assembly
                .CreateInstance($"{InputProgramEntryNamespace}.{InputProgramEntryClass}") 
                ?? throw new ArgumentException(
                    $"Unable to create instance of '{InputProgramEntryClass}' at {InputProgramEntryNamespace} from input solution");

            var methodInfo = entryClassInstance
                .GetType()
                .GetMethod(InputProgramEntryMethod)
                ?? throw new ArgumentException(
                    $"Unable to invoke '{InputProgramEntryMethod}' at {InputProgramEntryClass} from input solution");

            using var sw = new StringWriter();
            Console.SetOut(sw);
            methodInfo.Invoke(null, null);

            return sw.ToString().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public ExecutionResult CompileAndExecute(string code)
        {
            if (code is null)
                throw new ArgumentException("Expected code string value, got null");

            var compilation = GetCompilation(code);

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            var errors = new List<CompilationError>();
            IEnumerable<string> output = new List<string>();

            if (!emitResult.Success)
                foreach (var diagnostic in GetDiagnostics(emitResult))
                    errors.Add(new CompilationError(diagnostic));
            else
                output = RunAssemblyFromStream(ms);

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
            Dependencies.Select(refString =>
                    MetadataReference
                        .CreateFromFile(Path.Combine(_assemblyPath, refString)))
                .ToArray<MetadataReference>();
    }
}
