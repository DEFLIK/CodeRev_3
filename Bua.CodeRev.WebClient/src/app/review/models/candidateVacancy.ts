export enum CandidateVacancy {
    'juniorCS' = 'Junior C# developer',
    'middleCS' = 'Middle C# developer',
    'seniorCS' = 'Senior C# developer',
    'test' = 'Testy developer'

}

export type CandidateVacancyKeys = keyof typeof CandidateVacancy