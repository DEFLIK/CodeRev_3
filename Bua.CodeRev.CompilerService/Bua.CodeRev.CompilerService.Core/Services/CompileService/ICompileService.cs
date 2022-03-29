using System;
using System.Collections.Generic;

namespace Bua.CodeRev.CompilerService.Core.Services.CompileService
{
    public interface ICompileService
    {
        /// <summary>
        /// Ассинхронно производит компиляцию кода
        /// </summary>
        /// <param name="code">Строка кода для компиляции</param>
        /// <returns>Текстовый результат компиляции (подобно консольному выводу)</returns>
        /// <exception cref="ArgumentException">Переданная строка имеет значение null</exception>
        public IEnumerable<string> Compile(string code);
    }
}
