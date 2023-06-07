export class CandidateFilterCriteria {
    constructor(
        public expires: boolean,
        public descending: boolean,
        public states: string[],
        public vacancies: string[],
        public programmingLanguages: string[]) {}
}
