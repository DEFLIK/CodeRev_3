using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bua.CodeRev.CompilerService.Core.Models
{
    public class EntryPoint
    {
        public string Namespace { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
    }
}
