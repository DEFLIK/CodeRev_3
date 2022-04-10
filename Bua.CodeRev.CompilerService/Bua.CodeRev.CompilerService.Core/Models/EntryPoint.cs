using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bua.CodeRev.CompilerService.Core.Models
{
    public class EntryPoint
    {
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}
