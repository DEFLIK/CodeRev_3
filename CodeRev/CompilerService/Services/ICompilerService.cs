using System;
using CompilerService.Models;

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