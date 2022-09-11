using System.Linq;
using CompilerService.Helpers;
using CompilerService.Models;
using NUnit.Framework;

namespace Tests
{
    public class CompilerServiceTest
    {
        private static readonly EntryPoint _entryPoint = new()
        {
            NamespaceName = "CodeRevSolution",
            ClassName = "Program",
            MethodName = "Main"
        };
        private Compiler compiler;

        [SetUp]
        public void SetUp()
        {
            compiler = new Compiler();
        }

        [TestCase("hi")]
        [TestCase("Привет бобикам!")]
        public void Compile_ConsoleWriteLine_ShouldReturnText(string text)
        {
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
                }", _entryPoint);

            Assert.AreEqual(1, actual.Output.Count());
            Assert.AreEqual(text, actual.Output.First());
        }

        [Test]
        public void Compile_UseWrongLibrary_ShouldReturnError()
        {
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
                }", _entryPoint);

            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expected, actual.Errors.First().ErrorCode);
        }

        [Test]
        public void Compile_UseUnassignedVariable_ShouldReturnError()
        {
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
                }", _entryPoint);

            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expected, actual.Errors.First().ErrorCode);
        }
    }
}