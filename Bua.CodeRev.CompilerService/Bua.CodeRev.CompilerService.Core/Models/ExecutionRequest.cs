using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bua.CodeRev.CompilerService.Core.Models
{
    public class ExecutionRequest
    {
        public EntryPoint EntryPoint { get; set; }
        public string Code { get; set; }
    }
}
