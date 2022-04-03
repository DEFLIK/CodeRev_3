using System.Linq;
using Bua.CodeRev.CompilerService.Core.Services.CompileService;
using NUnit.Framework;

namespace Bua.CodeRev.CompilerService.Tests
{
    public class Tests
    {

        [TestCase("hi")]
        [TestCase("Привет бобикам!")]
        public void Compile_ConsoleWriteLine_ShouldReturnText(string text)
        {
            var compiler = new CompileService();

            var actual = compiler.CompileAndExecute(@"
                using System;
                namespace CodeRevSolution
                {
                    public class Program
                    {
                        public static void Main()
                        {
                            Console.WriteLine(""" + text + @""");
                        }
                    }
                }");

            Assert.AreEqual(1, actual.Output.Count());
            Assert.AreEqual(text, actual.Output.First());
        }

        [Test]
        public void Compile_UseWrongLibrary_ShouldReturnError()
        {
            var compiler = new CompileService();
            var expected = "CS0246";

            var actual = compiler.CompileAndExecute(@"
                using System123;
                namespace CodeRevSolution
                {
                    public class Program
                    {
                        public static void Main()
                        {
                        }
                    }
                }");

            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expected, actual.Errors.First().ErrorCode);
        }

        [Test]
        public void Compile_UseUnassignedVariable_ShouldReturnError()
        {
            var compiler = new CompileService();
            var expected = "CS0103";

            var actual = compiler.CompileAndExecute(@"
                using System;
                namespace CodeRevSolution
                {
                    public class Program
                    {
                        public static void Main()
                        {
                            Console.WriteLine(bruh);
                        }
                    }
                }");

            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expected, actual.Errors.First().ErrorCode);
        }
    }
}