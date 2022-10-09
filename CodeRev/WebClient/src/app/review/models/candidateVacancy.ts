/* eslint-disable @typescript-eslint/naming-convention */
export enum CandidateVacancy {
    'juniorCS' = 'Junior C# Developer',
    'middleCS' = 'Middle C# Developer',
    'seniorCS' = 'Senior C# Developer'
}

export type CandidateVacancyKeys = keyof typeof CandidateVacancy