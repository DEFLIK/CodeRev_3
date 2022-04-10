using System;
using System.Collections.Generic;
using Bua.CodeRev.CompilerService.Core.Models;

namespace Bua.CodeRev.CompilerService.Core.Services.CompileService
{
    public interface ICompileService
    {
        /// <summary>
        /// Ассинхронно производит выполнение кода
        /// </summary>
        /// <param name="code">Код в виде Plain-Text</param>
        /// <returns>Результат выполнения (подобно консольному выводу/ошибке)</returns>
        /// <exception cref="ArgumentException">Переданная строка имеет значение null</exception>
        public ExecutionResult Execute(string code, EntryPoint entryPoint);
    }
}
