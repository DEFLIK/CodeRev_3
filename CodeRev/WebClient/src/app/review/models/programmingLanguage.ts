export enum ProgrammingLanguage {
    csharp = 0,
    javaScript = 1,
}

export function convertProgrammingLanguageToString(language: ProgrammingLanguage): string {
    switch (language) {
        case ProgrammingLanguage.csharp:
            return 'C#';
        case ProgrammingLanguage.javaScript:
            return 'JavaScript';
        default:
            return 'Не указан';
    }
}
