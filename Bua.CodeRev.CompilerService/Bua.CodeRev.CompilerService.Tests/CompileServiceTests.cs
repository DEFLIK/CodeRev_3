using System.Linq;
using Bua.CodeRev.CompilerService.Core.Models;
using Bua.CodeRev.CompilerService.Core.Services.CompileService;
using NUnit.Framework;

namespace Bua.CodeRev.CompilerService.Tests
{
    public class Tests
    {
        private static EntryPoint EntryPoint = new EntryPoint()
        {
            NamespaceName = "CodeRevSolution",
            ClassName = "Program",
            MethodName = "Main"
        };

        [TestCase("hi")]
        [TestCase("Привет бобикам!")]
        public void Compile_ConsoleWriteLine_ShouldReturnText(string text)
        {
            var compiler = new CompileService();

            var actual = compiler.Execute(@"
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
                }", EntryPoint);

            Assert.AreEqual(1, actual.Output.Count());
            Assert.AreEqual(text, actual.Output.First());
        }

        [Test]
        public void Compile_UseWrongLibrary_ShouldReturnError()
        {
            var compiler = new CompileService();
            var expected = "CS0246";

            var actual = compiler.Execute(@"
                using System123;
                namespace CodeRevSolution
                {
                    public class Program
                    {
                        public static void Main()
                        {
                        }
                    }
                }", EntryPoint);

            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expected, actual.Errors.First().ErrorCode);
        }

        [Test]
        public void Compile_UseUnassignedVariable_ShouldReturnError()
        {
            var compiler = new CompileService();
            var expected = "CS0103";

            var actual = compiler.Execute(@"
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
                }", EntryPoint);

            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expected, actual.Errors.First().ErrorCode);
        }
    }
}