namespace UserService.DAL.Models.Enums;

public enum ProgrammingLanguage
{
    CSharp = 0,
    JavaScript = 1,
}

public static class ProgrammingLanguageExtensions
{
    public static string ToReadableString(this ProgrammingLanguage language)
        => language switch
        {
            ProgrammingLanguage.CSharp => "C#",
            ProgrammingLanguage.JavaScript => "JavaScript",
            _ => "Не указан"
        };
}