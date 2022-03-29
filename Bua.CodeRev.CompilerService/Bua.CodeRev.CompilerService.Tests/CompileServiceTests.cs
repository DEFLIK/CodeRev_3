using System;
using System.Linq;
using System.Threading.Tasks;
using Bua.CodeRev.CompilerService.Core.Services;
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

            var actual = compiler.Compile(@"
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
                }").First();

            Assert.AreEqual(text, actual.Trim());
        }

        [Test]
        public void Compile_UseWrongLibrary_ShouldReturnError()
        {
            var compiler = new CompileService();
            var expected = "CS0246:";

            var actual = compiler.Compile(@"
                using System123;
                namespace CodeRevSolution
                {
                    public class Program
                    {
                        public static void Main()
                        {
                            Console.WriteLine("">:("");
                        }
                    }
                }").First();

            Assert.AreEqual(expected, actual.Substring(0, 7));
        }

        [Test]
        public void Compile_UseUnassignedVariable_ShouldReturnError()
        {
            var compiler = new CompileService();
            var expected = "CS0103:";

            var actual = compiler.Compile(@"
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
                }").First();

            Assert.AreEqual(expected, actual.Substring(0, 7));
        }
    }
}