import { CandidateState } from './candidateState';
import { CandidateVacancy } from './candidateVacancy';

export class CandidateFitlerCriteria {

    constructor(
        public expires: boolean,
        public descending: boolean,
        public states: string[],
        public vacancies: string[]) {}
}