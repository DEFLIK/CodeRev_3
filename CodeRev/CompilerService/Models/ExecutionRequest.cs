using UserService.DAL.Models.Enums;

namespace CompilerService.Models
{
    public class ExecutionRequest
    {
        public EntryPoint EntryPoint { get; set; }
        public string Code { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}
