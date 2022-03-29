using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private MetadataReference[] GetMetadataReferences() => 
            Dependencies.Select(refString => 
                MetadataReference
                    .CreateFromFile(Path.Combine(_assemblyPath, refString)))
                .ToArray<MetadataReference>();

        private static IEnumerable<Diagnostic> GetDiagnostics(EmitResult emitResult) =>
            emitResult
                .Diagnostics
                .Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

        /// <returns>Консольный вывод запущенной сборки</returns>
        private static string RunAssemblyFromStream(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var entryClassInstance = assembly
                .CreateInstance($"{InputProgramEntryNamespace}.{InputProgramEntryClass}")
                ?? throw new NullReferenceException(
                    $"Unable to create instance of '{InputProgramEntryClass}' at {InputProgramEntryNamespace} from input solution");

            var methodInfo = entryClassInstance
                .GetType()
                .GetMethod(InputProgramEntryMethod) 
                ?? throw new NullReferenceException(
                    $"Unable to invoke '{InputProgramEntryMethod}' at {InputProgramEntryClass} from input solution");

            using var sw = new StringWriter();
            Console.SetOut(sw);
            methodInfo.Invoke(null, null);

            return sw.ToString();
        }

        public CompileService()
        {
            _assemblyPath = Path
                .GetDirectoryName(typeof(object).Assembly.Location) 
                ?? throw new NullReferenceException("Failed to create CompileService instance: unable to get assembly location");
        }


        public IEnumerable<string> Compile(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assemblyName = Path.GetRandomFileName();
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: GetMetadataReferences(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
                foreach (var diagnostic in GetDiagnostics(emitResult))
                    yield return $"{diagnostic.Id}: {diagnostic.GetMessage()}";
            else
                yield return RunAssemblyFromStream(ms);
        }
    }
}
