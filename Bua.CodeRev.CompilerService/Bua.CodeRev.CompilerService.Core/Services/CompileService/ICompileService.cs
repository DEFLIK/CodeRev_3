using System;
using System.Collections.Generic;
using Bua.CodeRev.CompilerService.Core.Models;

namespace Bua.CodeRev.CompilerService.Core.Services.CompileService
{
    public interface ICompileService
    {
        /// <summary>
        /// Ассинхронно производит компиляцию кода
        /// </summary>
        /// <param name="code">Строка кода для компиляции</param>
        /// <returns>Результат компиляции (подобно консольному выводу/ошибке)</returns>
        /// <exception cref="ArgumentException">Переданная строка имеет значение null</exception>
        public ExecutionResult CompileAndExecute(string code);
    }
}
